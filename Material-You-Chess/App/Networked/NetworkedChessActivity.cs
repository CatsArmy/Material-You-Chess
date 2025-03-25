using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.Extensions;
using Chess.App.Nearby;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Player;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Locked,
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class NetworkedChessActivity : LobbyBottomSheet, IChessActivity
{
    public Context? Context => this;

    public required ChessGame Game { get; set; }
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

    public required UserClient Client { get; set; }
    public required UserClient ConnectedClient { get; set; }

    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName => FirebaseAuth.Instance.CurrentUser?.Uid
        ?? throw new("No Uid for the current user?");
    protected override Strategy Strategy => Strategy.P2pStar;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (!this.MaterialYouThemePreference())
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
        var ClientName = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        if (this.State == State.Advertising)
        {
#if DEBUG
            Logger.Verbose("Client is white player");
#endif
            var client = new WhitePlayerClient(this.AdvertisingName, ClientName ?? "White Player");
            client.LoadProfilePicture(Glide.With(this)).Into(this.WhitePlayerProfilePicture!);
            this.WhitePlayerUsername!.Text = client!.Username;
            this.Client = client;

            //Init handshake
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(client, SourceJsonGenerationContext.Default.FirebaseUserClient)));
        }

        if (this.State == State.Discovering)
        {
#if DEBUG
            Logger.Verbose("Client is black player");
#endif
            var client = new BlackPlayerClient(this.AdvertisingName, ClientName ?? "Black Player");
            client.LoadProfilePicture(Glide.With(this)).Into(this.BlackPlayerProfilePicture!);
            this.BlackPlayerUsername!.Text = client.Username;
            this.Client = client;

            //Init handshake
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(this.Client, SourceJsonGenerationContext.Default.FirebaseUserClient)));
        }

        this.State = State.Idle;
    }

    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        if (this.Game != null)
        {
            Toast.MakeText(this, $"Error, {this.Client!.Username} disconnected", ToastLength.Short)?.Show();
            this.SetResult(Result.Canceled);
            this.Finish();
        }
    }

    /// <summary>
    /// Allows <see cref="IChessActivity"/> access to <see cref="ConnectionsActivity.Send(Payload)"/>
    /// by both implementing <see cref="IChessActivity.Send(Payload)"/> 
    /// and overriding the methods <see cref="ConnectionsActivity.Send(Payload)"/>
    /// </summary>
    public override void Send(Payload payload) => base.Send(payload);

    /// <summary> Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client </summary>
    /// <param name="endpoint">The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload">The <see cref="Payload"/> containing all the data for us to handle the event </param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType != Payload.Type.Bytes)
            return;

        if (this.Game != null)
        {
            var move = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.Move);
            this.Game.PlayMove(move!, false);
            return;
        }

        this.StandardBottomSheet!.Visibility = ViewStates.Gone;
        this.BottomSheet!.RemoveBottomSheetCallback(this.Callback!);
        switch (JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.FirebaseUserClient))
        {
            case WhitePlayerClient whiteClient:
                this.WhitePlayerUsername!.Text = whiteClient.Username;
                whiteClient.LoadProfilePicture(Glide.With(this)).Into(this.WhitePlayerProfilePicture!);
                this.ConnectedClient = whiteClient;
                this.Game = new ChessGame(this);
                break;

            case BlackPlayerClient blackClient:
                this.BlackPlayerUsername!.Text = blackClient.Username;
                blackClient.LoadProfilePicture(Glide.With(this)).Into(this.BlackPlayerProfilePicture!);
                this.ConnectedClient = blackClient;
                this.Game = new ChessGame(this);
                break;

            default:
                Logger.Warn("Unknown state something went wrong");
                break;
        }
    }
}
