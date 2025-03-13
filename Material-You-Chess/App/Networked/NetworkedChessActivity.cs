using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.App.Nearby;
using Chess.Dialogs;
using Chess.Game;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked)]
public class NetworkedChessActivity : LobbyBottomSheet, IChessActivity
{
    public ChessGame? Game { get; set; }
    public Context? Context { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? Player1ShapeableImageView { get; set; }
    public ShapeableImageView? Player2ShapeableImageView { get; set; }
    public TextView? Profile1Username { get; set; }
    public TextView? Profile2Username { get; set; }
    public LobbyBottomSheet? LobbyWaitingRoom;
    public State State
    {
        get; set
        {
            this.OnStateChanged(field, value);
            field = value;
        }
    } = State.Idle;

    public string Player1Name { get; private set; } = string.Empty;
    public string Player2Name { get; private set; } = string.Empty;

    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string Name => FirebaseAuth.Instance.CurrentUser!.DisplayName!;
    protected override Strategy Strategy => Strategy.P2pStar;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"), out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        this.Player1ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.Player2ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);

        this.Profile1Username = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.Profile2Username = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
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

    public override void Send(Payload payload) => base.Send(payload);

    public void OnStateChanged(State currentState, State requestedState)
    {
        if (currentState == requestedState)
            return;

        if (currentState == State.Advertising)
        {
            this.StopAdvertising();
            Thread.Sleep(10);
        }

        if (currentState == State.Discovering)
        {
            this.StopDiscovering();
            Thread.Sleep(10);
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
        if (!this.IsConnecting)
            this.ConnectToEndpoint(endpoint);
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo)
    {
        if (!this.IsConnecting)
            this.AcceptConnection(endpoint);
    }

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Found opponent, {endpoint.Name}", ToastLength.Short)?.Show();
        switch (this.State)
        {
            case State.Advertising:
                Logger.Error("isConnection Initiator true");
                this.Profile1Username!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
                };
                this.Profile2Username!.Text = endpoint.Name;
                this.Player2Name = endpoint.Name;
                this.Player1Name = FirebaseAuth.Instance?.CurrentUser?.DisplayName!;
                this.Game = new ChessGame(this, true);
                break;

            case State.Discovering:
                Logger.Error("isConnection Initiator false");
                this.Player2Name = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName!,
                };

                this.Profile2Username!.Text = this.Player2Name;
                this.Player1Name = endpoint.Name;
                this.Profile1Username!.Text = this.Player1Name;

                this.Game = new ChessGame(this, false);
                break;
        }
    }

    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Error, {endpoint.Name} disconnected", ToastLength.Short)?.Show();
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {

    }

    /// <summary> Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client </summary>
    /// <param name="endpoint">The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload">The <see cref="Payload"/> containing all the data for us to handle the event </param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var move = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.Move);
            move!.Origin.Move(move, this.Game!);
        }
    }
}
