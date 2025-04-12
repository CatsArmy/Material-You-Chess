using System.Text.Json;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Motion.Widget;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.Game;
using Chess.Game.Common;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked.Nearby;

[Activity(
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked,
    EnableOnBackInvokedCallback = true
)]
public partial class NetworkedChessActivity : IChessActivity
{
    protected static readonly NullReferenceException UserIsNull = new("CurrentUser is somehow null???");
    protected readonly FirebaseUser CurrentUser = FirebaseAuth.Instance.CurrentUser ?? throw UserIsNull;
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName => this.CurrentUser.Uid;
    protected override Strategy Strategy => Strategy.P2pStar;
    public ChessBottomSheet? BottomSheet { get; set; }
    public required ChessGame Game { get; set; }
    public required UserClient Client { get; set; }
    public required UserClient ConnectedClient { get; set; }

    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    private ConnectionsClientState State
    {
        get; set
        {
            this.OnStateChanged(field, value);
            field = value;
        }
    } = ConnectionsClientState.Idle;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        IChessActivity.Instance = this;
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity);
        this.PromotionDialogs = (new(this), new(this));
        this.WhitePlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.whitePlayerProfilePicture);
        this.BlackPlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.blackPlayerProfilePicture);
        this.WhitePlayerUsername = base.FindViewById<TextView>(Resource.Id.whitePlayerUsername);
        this.BlackPlayerUsername = base.FindViewById<TextView>(Resource.Id.blackPlayerUsername);
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.BottomSheet = ChessBottomSheet.OnCreate(this);
        this.BottomSheet.Show();
    }

    protected override void OnDestroy()
    {
        IChessActivity.Instance = null;
        Logger.Debug($"{nameof(OnDestroy)}");
        base.OnDestroy();
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo) => this.AcceptConnection(endpoint);

    protected override void OnConnectionFailed(EndPoint endpoint) => this.StartDiscovering();

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        var firebaseUserClient = this.CurrentUser;
        if (this.State == ConnectionsClientState.Advertising)
        {
            Logger.Verbose("Client is white player");
            var client = new WhitePlayerClient(firebaseUserClient!);
            client.LoadProfilePicture(Glide.With(this)).Placeholder(this.WhitePlayerProfilePicture!.Drawable!)
                .Into(this.WhitePlayerProfilePicture!);
            this.WhitePlayerUsername!.Text = client!.Username;
            this.Client = client;
        }

        if (this.State == ConnectionsClientState.Discovering)
        {
            Logger.Verbose("Client is black player");
            var client = new BlackPlayerClient(firebaseUserClient!);
            client.LoadProfilePicture(Glide.With(this)).Placeholder(this.BlackPlayerProfilePicture!.Drawable!)
                .Into(this.BlackPlayerProfilePicture!);
            this.BlackPlayerUsername!.Text = client.Username;
            this.Client = client;
        }

        //Init handshake between the 2 devices and stop discovering and advertising for other devices
        this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(this.Client, SourceJsonGenerationContext.Default.FirebaseUserClient)));
        this.State = ConnectionsClientState.Idle;
    }

    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        if (this.Game == null) return;

        Toast.MakeText(this, $"Error, {this.Client!.Username} disconnected", ToastLength.Short)?.Show();
        var description = $"{this.ConnectedClient.Username} disconnected, {this.Client.Username} wins by technicality";
        switch (this.Game.ClientIsWhite) // Inverse because our client did not disconnect
        {
            case true: //our client is white and the connect client is black
                this.BottomSheet?.ShowGameOver(this.Game.BlackPlayer!, description);
                break;

            default:
                this.BottomSheet?.ShowGameOver(this.Game.WhitePlayer!, description);
                break;
        }
    }

    /// <summary> We found an advertiser! </summary>
    /// <param name="endpoint">the endpoint of the advertiser we discovered</param>
    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        this.StopDiscovering();
        this.ConnectToEndpoint(endpoint);
    }

    /// <summary> Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client </summary>
    /// <param name="endpoint"> The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload"> The <see cref="Payload"/> containing all the data for us to handle the event </param>
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

        this.BottomSheet!.Callback.ToState = null;
        this.BottomSheet!.Behavior.State = (int)VisibilityState.Collapsed;
        switch (JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.FirebaseUserClient))
        {
            case WhitePlayerClient whiteClient:
                this.ConnectedClient = whiteClient;
                this.WhitePlayerUsername!.Text = whiteClient.Username;
                whiteClient.LoadProfilePicture(Glide.With(this))
                    .Placeholder(this.WhitePlayerProfilePicture!.Drawable!)
                    .Into(this.WhitePlayerProfilePicture!);
                this.Game = new ChessGame(this);
                return;

            case BlackPlayerClient blackClient:
                this.ConnectedClient = blackClient;
                this.BlackPlayerUsername!.Text = blackClient.Username;
                blackClient.LoadProfilePicture(Glide.With(this))
                    .Placeholder(this.BlackPlayerProfilePicture!.Drawable!)
                    .Into(this.BlackPlayerProfilePicture!);
                this.Game = new ChessGame(this);
                return;

            default:
                break;
        }
        Logger.Warn("Unknown state something went wrong");
    }

    /// <summary> makes sure the last state is no longer active and activate the new state </summary>
    private void OnStateChanged(ConnectionsClientState currentState, ConnectionsClientState requestedState)
    {
        if (currentState == requestedState) return;

        switch (currentState)
        {
            case ConnectionsClientState.Advertising:
                StopAdvertising();
                break;
            case ConnectionsClientState.Discovering:
                StopDiscovering();
                break;
        }

        switch (requestedState)
        {
            case ConnectionsClientState.Idle:
                StopAdvertising();
                StopDiscovering();
                break;
            case ConnectionsClientState.Advertising:
                StartAdvertising();
                break;
            case ConnectionsClientState.Discovering:
                StartDiscovering();
                break;
        }
    }

    public void OnSelectNone()
    {
        this.BottomSheet!.SearchingIndicator?.Hide();
        this.BottomSheet!.SearchingText!.Text = "Please select a matchmaking preference";
        this.State = ConnectionsClientState.Idle;
    }

    public void OnSelectWhite()
    {
        this.BottomSheet!.SearchingIndicator?.Show();
        this.BottomSheet!.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
        this.State = ConnectionsClientState.Advertising;
    }

    public void OnSelectBlack()
    {
        this.BottomSheet!.SearchingIndicator?.Show();
        this.BottomSheet!.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
        this.State = ConnectionsClientState.Discovering;
    }

    private enum ConnectionsClientState
    {
        Idle,
        Advertising,
        Discovering
    }
}
