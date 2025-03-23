using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.App.Nearby;
using Chess.Dialogs;
using Chess.Game;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Locked,
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class NetworkedChessActivity : LobbyBottomSheet, IChessActivity
{
    public ChessGame? Game { get; set; }
    public Context? Context { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }

    public State State
    {
        get; set
        {
            this.OnStateChanged(field, value);
            field = value;
        }
    } = State.Idle;

    public string? ClientName;
    public string? ConnectedClientName;

    public string WhitePlayerName
    {
        get; private set
        {
            field = value;
            this.WhitePlayerUsername!.Text = value;
        }
    } = "White Player";

    public string BlackPlayerName
    {
        get; private set
        {
            field = value;
            this.BlackPlayerUsername!.Text = value;
        }
    } = "Black Player";

    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName => FirebaseAuth.Instance.CurrentUser?.Uid
        ?? throw new("No Uid for the current user?");
    protected override Strategy Strategy => Strategy.P2pStar;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"),
            out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        this.WhitePlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);

        this.WhitePlayerUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));
        this.BoardLayout = this.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        base.OnCreate();
    }

    public override void OnSelectWhite()
    {
        base.OnSelectWhite();
        this.State = State.Advertising;
    }

    public override void OnSelectBlack()
    {
        base.OnSelectBlack();
        this.State = State.Discovering;
    }

    public override void OnSelectNone()
    {
        base.OnSelectNone();
        this.State = State.Idle;
    }

    public void OnStateChanged(State currentState, State requestedState)
    {
        if (currentState == requestedState)
            return;

        if (currentState == State.Advertising)
        {
            this.StopAdvertising();
        }

        if (currentState == State.Discovering)
        {
            this.StopDiscovering();
        }

        if (requestedState == State.Idle)
        {
            this.StopAdvertising();
            this.StopDiscovering();
        }

        if (requestedState == State.Advertising)
        {
            this.StartAdvertising();
        }

        if (requestedState == State.Discovering)
        {
            this.StartDiscovering();
        }
    }

    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        //We found an advertiser!
        this.StopDiscovering();
        this.ConnectToEndpoint(endpoint);
    }

    protected override void OnConnectionFailed(EndPoint endpoint) => this.StartDiscovering();

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo) => this.AcceptConnection(endpoint);

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
#if DEBUG
        Toast.MakeText(this, $"DEBUG: Connected to Client: {{id}}::{endpoint.Name}", ToastLength.Short)?.Show();
#endif
        this.ClientName = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        if (this.State == State.Advertising)
        {
#if DEBUG
            Logger.Verbose("Client is white player");
#endif
            var client = new WhiteClient(this.AdvertisingName, this.ClientName ?? this.WhitePlayerName);
            client.LoadProfilePicture(Glide.With(this)).Into(this.WhitePlayerProfilePicture!);
            this.WhitePlayerName = client!.WhitePlayerName;

            //Init handshake
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(client, SourceJsonGenerationContext.Default.UserClient)));
        }

        if (this.State == State.Discovering)
        {
#if DEBUG
            Logger.Verbose("Client is black player");
#endif
            var client = new BlackClient(this.AdvertisingName, this.ClientName ?? this.WhitePlayerName);
            client.LoadProfilePicture(Glide.With(this)).Into(this.BlackPlayerProfilePicture!);
            this.BlackPlayerName = client.BlackPlayerName;

            //Init handshake
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(client, SourceJsonGenerationContext.Default.UserClient)));
        }

        this.State = State.Idle;
    }

    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        if (this.Game != null)
        {
            Toast.MakeText(this, $"Error, {this.ClientName} disconnected", ToastLength.Short)?.Show();
            this.SetResult(Result.Canceled, new Intent());
            this.Finish();
        }
    }

    /// <summary>
    /// Gives IChessActivity the send command by both implementing and overriding the methods
    /// </summary>
    public override void Send(Payload payload) => base.Send(payload);

    /// <summary> Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client </summary>
    /// <param name="endpoint">The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload">The <see cref="Payload"/> containing all the data for us to handle the event </param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType != Payload.Type.Bytes)
        {
            return;
        }

        if (this.Game != null)
        {
            var move = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.Move);
            move!.Origin.Move(move, this.Game!);
            return;
        }

        this.StandardBottomSheet!.Visibility = ViewStates.Gone;
        this.BottomSheet!.RemoveBottomSheetCallback(this.Callback!);
        var otherClient = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.UserClient);

        if (otherClient is WhiteClient whiteClient)
        {
            this.ConnectedClientName = whiteClient.WhitePlayerName;
            this.WhitePlayerName = this.ConnectedClientName;
            whiteClient.LoadProfilePicture(Glide.With(this)).Into(this.WhitePlayerProfilePicture!);
            this.Game = new ChessGame(this, false);
            return;
        }

        if (otherClient is BlackClient blackClient)
        {
            this.ConnectedClientName = blackClient.BlackPlayerName;
            this.BlackPlayerName = this.ConnectedClientName;
            blackClient.LoadProfilePicture(Glide.With(this)).Into(this.BlackPlayerProfilePicture!);
            this.Game = new ChessGame(this, true);
            return;
        }

        Logger.Warn("Unknown state something went wrong");
    }
}
