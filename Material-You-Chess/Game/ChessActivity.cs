using System.Text.Json;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Bumptech.Glide;
using Firebase;
using Firebase.AppCheck;
using Firebase.AppCheck.PlayIntegrity;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Material.You.Chess.App;
using Material.You.Chess.App.Common;
using Material.You.Chess.Game.Common;
using Material.You.Chess.Game.Networked;
using Material.You.Chess.Game.Player;
using Microsoft.Maui.ApplicationModel;

namespace Material.You.Chess.Game;

[Activity(
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked,
    EnableOnBackInvokedCallback = true
)]
public class ChessActivity : Networked.Nearby.ConnectionsActivity
{
    /// <summary> static activity variable that is required for json deserialization </summary>
    public static ChessActivity? Instance { get; private set; }
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    /// <summary>
    /// The connection strategy im using for Nearby Connections is P2pStar(N:1),
    /// which is a combination of Bluetooth Classic and WiFi Hotspots.
    /// </summary>
    protected override Strategy Strategy => Strategy.P2pPointToPoint;

    /// <summary>
    /// The name used when advertising this device.
    /// using json for filtering the connections we accept
    /// </summary>
    protected override string AdvertisingName => JsonSerializer.Serialize(this.Player, SourceJsonGenerationContext.Default.NetworkedPlayer);
    public Networked.Player? Player { get; set; }
    public Networked.Player? ConnectedClient { get; set; }
    public required ChessGame Game { get; set; }
    public required User User { get; set; }

    public required FirebaseAuth Auth { get; set; }
    public ChessBottomSheet? BottomSheet { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public bool IsNetworked { get; private set; } = false;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        ChessActivity.Instance = this;
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity);
        this.WhitePlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.whitePlayerProfilePicture);
        this.BlackPlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.blackPlayerProfilePicture);
        this.WhitePlayerUsername = base.FindViewById<TextView>(Resource.Id.whitePlayerUsername);
        this.BlackPlayerUsername = base.FindViewById<TextView>(Resource.Id.blackPlayerUsername);
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.BottomSheet = ChessBottomSheet.OnCreate(this, this.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet)!);
        if (this.Intent!.GetStringExtra(MainFragment.IsNetworked) is not string value)
            return;

        this.IsNetworked = bool.Parse(value);
        if (this.IsNetworked)
        {
            var app = FirebaseApp.InitializeApp(this)!;
            var check = FirebaseAppCheck.GetInstance(app);
            check.InstallAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.Instance);
            this.Auth = FirebaseAuth.GetInstance(app);
            if (this.Auth.CurrentUser is FirebaseUser user)
            {
                this.User = new User(user);
                this.BottomSheet.ShowMatchmaking();
            }
        }
        else
        {
            var white = new Player.Player(new($"{nameof(White)} {nameof(Player)}"), true);
            this.WhitePlayerUsername!.Text = white.User.Name;
            var black = new Player.Player(new($"{nameof(Black)} {nameof(Player)}"), false);
            this.BlackPlayerUsername!.Text = black.User.Name;
            this.Game = new(this, white, black);
        }
    }

    protected override void OnDestroy()
    {
        ChessActivity.Instance = null;
        base.OnDestroy();
    }

    /// <summary> We found an advertiser, lets try to connect to it </summary>
    /// <remarks> Called when a remote endpoint is discovered, not to be confused 
    /// with ConnectToEndpoint(EndPoint) which is called when connecting to the device </remarks>
    /// <param name="remote">the endpoint of the advertiser we discovered</param>
    protected override void OnEndpointDiscovered(EndPoint remote) => this.ConnectToEndpoint(remote); // Request connection

    /// <summary> 
    /// Called when a pending connection with a remote endpoint is created. 
    /// if we want to continue with the connection, call AcceptConnection(Endpoint). 
    /// Otherwise, RejectConnection(Endpoint). must respond using the methods above
    /// </summary>
    /// <remarks> this code is call for both this device and the remote(other) device </remarks>
    protected override async void OnConnectionInitiated(EndPoint remote, ConnectionInfo connectionInfo)
    {
        string json = remote.Name;
        var otherClient = JsonSerializer.Deserialize(json, SourceJsonGenerationContext.Default.NetworkedPlayer);
        // prevent connecting to more than one device and apply the matchmaking preference
        if (this.IsConnected || this.Player?.IsWhite == otherClient?.IsWhite)
        {
            await this.RejectConnection(remote);
            this.IsConnecting = false;
            return;
        }
        // we Accept our end of the symmetric connection api
        this.IsConnecting = true;
        this.ConnectedClient = otherClient;
        await this.AcceptConnection(remote);
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {
        this.ConnectedClient = null;
        this.IsDiscovering = false;
        this.IsAdvertising = false;
        this.BottomSheet?.White.Checked = false;
        this.BottomSheet?.Black.Checked = false;
        this.BottomSheet?.OnSelectNone(this);
        Toast.MakeText(this, "Connection Error: reselect your preferences", ToastLength.Long);
        this.BottomSheet?.ShowMatchmaking();
    }

    protected override void OnDiscoveryFailed()
    {
        base.OnDiscoveryFailed();
        Toast.MakeText(this, "Discovery error, please reselect your preference", ToastLength.Long);
        this.BottomSheet?.OnSelectNone(this); // stop all some some is preventing us from discovering
    }

    protected override void OnAdvertisingFailed()
    {
        base.OnAdvertisingFailed();
        Toast.MakeText(this, "Advertising error, please reselect your preference", ToastLength.Long);
        this.BottomSheet?.OnSelectNone(this); // stop all some some is preventing us from advertising
    }

    /// <summary> 
    /// Called when someone has connected to us.
    /// Both ends of the connection have been accepted
    /// </summary>
    protected override void OnEstablishedConnection(EndPoint remote)
    {
        this.IsDiscovering = false;
        this.IsAdvertising = false;
        // prevent ourselves from looking for more devices to connect to. 
        this.BottomSheet!.Callback.ToState = null;
        this.BottomSheet!.Behavior.State = (int)VisibilityState.Collapsed;
        if (this.Player?.IsWhite is null || this.ConnectedClient?.IsWhite is null)
            return;

        if (this.Player.IsWhite && !this.ConnectedClient.IsWhite)
        {
            var (whiteUser, isWhite) = this.Player;
            this.WhitePlayerUsername!.Text = this.Player.FirebaseUser.Users.Name;
            this.Player.FirebaseUser.TryLoadPfP(Glide.With(this))?.Placeholder(this.WhitePlayerProfilePicture!.Drawable!)
                .Into(this.WhitePlayerProfilePicture!);

            var (blackUser, isBlack) = this.ConnectedClient;
            Logger.Debug(blackUser.Client.Name);
            this.BlackPlayerUsername!.Text = this.ConnectedClient.FirebaseUser.Users.Name;
            this.ConnectedClient.FirebaseUser.TryLoadPfP(Glide.With(this))
                ?.Placeholder(this.BlackPlayerProfilePicture!.Drawable!).Into(this.BlackPlayerProfilePicture!);

            this.Game = new(this, new(whiteUser.Client, isWhite),
                new(blackUser.Client, isBlack));
        }

        if (!this.Player.IsWhite && this.ConnectedClient.IsWhite)
        {
            var (whiteUser, isWhite) = this.ConnectedClient;
            this.WhitePlayerUsername!.Text = this.ConnectedClient.FirebaseUser.Users.Name;
            this.ConnectedClient.FirebaseUser.TryLoadPfP(Glide.With(this))
                ?.Placeholder(this.WhitePlayerProfilePicture!.Drawable!).Into(this.WhitePlayerProfilePicture!);
            Logger.Debug(whiteUser.Client.Name);

            var (blackUser, isBlack) = this.Player;
            this.BlackPlayerUsername!.Text = this.Player.FirebaseUser.Users.Name;
            this.Player.FirebaseUser.TryLoadPfP(Glide.With(this))
                ?.Placeholder(this.BlackPlayerProfilePicture!.Drawable!).Into(this.BlackPlayerProfilePicture!);

            this.Game = new(this, new(whiteUser.Client, isWhite),
                new(blackUser.Client, isBlack));
        }
    }

    /// <summary> Sends a Payload to all currently connected endpoints if the activity is networked </summary>
    /// <param name="payload">The data you want to send. </param>
    public override void Send(Payload payload)
    {
        if (this.IsNetworked)
            base.Send(payload);

        this.OnReceive(payload.AsBytes()!);
    }

    /// <summary>
    /// parse and play the move if we have a game instance otherwise 
    /// we can assume that we need to parse a player client
    /// </summary>
    /// <param name="payload"></param>
    private void OnReceive(byte[] payload)
    {
        if (this.Game is not null)
        {
            var move = JsonSerializer.Deserialize(payload, SourceJsonGenerationContext.Default.Move);
            this.Game.PlayMove(move!, false);
        }
    }

    /// <remarks> Someone who is connected to us has sent us data. Overridden this method to handle this event. </remarks>
    /// <summary> Handles the payload sent by endpoint client </summary>
    /// <param name="endpoint"> The client who is sending the payload to us </param>
    /// <param name="payload"> The Payload containing all the data for us to handle the event </param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
            this.OnReceive(payload.AsBytes()!);
    }

    /// <summary> Called when someone has disconnected. Overridden this method to inform the user about the event. </summary>
    protected override void OnEndpointDisconnected(EndPoint remote)
    {
        if (!this.IsNetworked) return;
        if (this.Game is null) // we have not yet started the game
        {
            this.OnConnectionFailed(remote); // reuse connection failure code
            return;
        }

        Toast.MakeText(this, $"Error, {this.User!.Client.Name} disconnected", ToastLength.Short)?.Show();
        var description = $"{this.ConnectedClient?.FirebaseUser.Client.Name} disconnected, {this.User!.Client.Name} wins by technicality";
        if (this.ConnectedClient?.IsWhite == true)
            this.BottomSheet?.ShowGameOver(this.Game.WhitePlayer!, description);
        if (this.ConnectedClient?.IsWhite == false)
            this.BottomSheet?.ShowGameOver(this.Game.BlackPlayer!, description);
    }
}
