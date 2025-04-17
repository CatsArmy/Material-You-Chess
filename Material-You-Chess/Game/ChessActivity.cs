using System.Text.Json;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Bumptech.Glide;
using Chess.App;
using Chess.App.Common;
using Chess.Game.Common;
using Chess.Game.Networked;
using Chess.Game.Networked.Nearby;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.Game;

[Activity(
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked,
    EnableOnBackInvokedCallback = true
)]
public class ChessActivity : ConnectionsActivity
{
    /// <summary> global activity variable that is required for json deserialization </summary>
    public static ChessActivity? Instance { get; private set; }
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName
    {
        get
        {
            var White = this.IsAdvertising;
            var Black = this.IsDiscovering;
            //if (White == Black)
            //    return this.Uid;

            //if (White)
            //    return $"{nameof(White)}::{this.Uid}";

            //if (Black)
            //    return $"{nameof(Black)}::{this.Uid}";

            return this.Uid;
        }
    }

    protected override Strategy Strategy => Strategy.P2pPointToPoint;
    public ChessBottomSheet? BottomSheet { get; set; }
    public required ChessGame Game { get; set; }

    /// <summary> The client of the player on this device </summary>
    public required IPlayerClient Client { get; set; }

    /// <summary> The client of the player that we connect to </summary>
    public required IPlayerClient ConnectedClient { get; set; }

    public ConstraintLayout? BoardLayout { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public bool IsNetworked { get; private set; } = false;
    private string? Username;
#nullable disable
    private string Uid
    {
        get; set
        {
            field = value;
            if (this.IsNetworked && (value is null || value == string.Empty))
                throw new NullReferenceException($"{nameof(AdvertisingName)} cannot be null when networking is on", IFirebaseUserClient.NullUid);
        }
    }
#nullable restore
    private string ThisClientStartsWith
    {
        get
        {
            if (this.IsDiscovering == this.IsAdvertising)
            {
                return string.Empty;
            }

            if (this.IsAdvertising)
            {
                return "White::";
            }
            if (this.IsDiscovering)
            {
                return "Black::";
            }

            return string.Empty;
        }
    }

    private string ConnectedClientStartsWith
    {
        get
        {
            if (this.IsDiscovering == this.IsAdvertising)
            {
                return string.Empty;
            }

            if (!this.IsAdvertising)
            {
                return "White::";
            }
            if (!this.IsDiscovering)
            {
                return "Black::";
            }
            return string.Empty;
        }
    }

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

        var args = this.Intent!.GetBundleExtra(MainFragment.IntentArgs);
        if (args is null) return;
        this.IsNetworked = args.GetBoolean(MainFragment.IsNetworked, this.IsNetworked);
        this.Username = args.GetString(MainFragment.Username);
        this.Uid = args.GetString(MainFragment.Uid);
        if (this.IsNetworked)
        {
            this.BottomSheet.ShowMatchmaking();
            return;
        }

        this.Client = new WhiteClient(this.Username ?? string.Empty, this.Uid ?? string.Empty);
        this.WhitePlayerUsername!.Text = this.Client.Username;
        var profilePicture = this.WhitePlayerProfilePicture!;
        this.Client.TryLoadProfilePicture(Glide.With(this))?.Placeholder(profilePicture.Drawable!).Into(profilePicture);
        // only send the handshake when we know the type of our client
        this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(this.Client, SourceJsonGenerationContext.Default.WhiteClient)));
    }

    protected override void OnDestroy()
    {
        ChessActivity.Instance = null;
        base.OnDestroy();
    }

    /// <summary> 
    /// Called when a pending connection with a remote endpoint is created. 
    /// we instantly accept the connection(FIFO)
    /// </summary>
    protected override async void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo)
    {
        if (endpoint.Name.StartsWith(this.ConnectedClientStartsWith))
            await this.AcceptConnection(endpoint);
    }

    /// <summary>
    /// Called when a connection with this endpoint has failed. 
    /// Overridden this method try searching for a different endpoint 
    /// </summary>
    protected override void OnConnectionFailed(EndPoint endpoint) => this.IsDiscovering = true;

    /// <summary> 
    /// Called when someone has connected to us.
    /// Overridden this method to start the handshake between the 2 devices 
    /// </summary>
    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        if (this.IsAdvertising)
        {
            var client = new WhiteClient(this.Username ?? string.Empty, this.Uid);
            this.Client = client;
            this.WhitePlayerUsername!.Text = this.Client.Username;
            var profilePicture = this.WhitePlayerProfilePicture!;
            this.Client.TryLoadProfilePicture(Glide.With(this))?.Placeholder(profilePicture.Drawable!).Into(profilePicture);
            // only send the handshake when we know the type of our client
            this.IsDiscovering = false; // prevent ourselves from trying to connect at the same time
            this.IsAdvertising = false; // prevent ourselves from trying to connect at the same time
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(client, SourceJsonGenerationContext.Default.WhiteClient)));

        }
        else if (this.IsDiscovering)
        {
            var client = new BlackClient(this.Username ?? string.Empty, this.Uid);
            this.Client = client;
            this.BlackPlayerUsername!.Text = this.Client.Username;
            var profilePicture = this.BlackPlayerProfilePicture!;
            this.Client.TryLoadProfilePicture(Glide.With(this))?.Placeholder(profilePicture.Drawable!).Into(profilePicture);
            // only send the handshake when we know the type of our client
            this.IsDiscovering = false; // prevent ourselves from trying to connect at the same time
            this.IsAdvertising = false; // prevent ourselves from trying to connect at the same time
            this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(client, SourceJsonGenerationContext.Default.BlackClient)));
        }
        // disconnect if the endpoint does not match our needs black+black / white+white / no longer advertising/discovering
        else this.Disconnect(endpoint);
    }

    /// <summary> We found an advertiser, lets try to connect to it </summary>
    /// <remarks> Called when a remote endpoint is discovered, not to be confused 
    /// with ConnectToEndpoint(EndPoint) which is called when connecting to the device </remarks>
    /// <param name="endpoint">the endpoint of the advertiser we discovered</param>
    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        if (endpoint.Name.StartsWith(this.ConnectedClientStartsWith))
        {
            this.ConnectToEndpoint(endpoint);
        }
    }

    /// <summary> Sends a Payload to all currently connected endpoints if the activity is networked </summary>
    /// <param name="payload">The data you want to send. </param>
    public override void Send(Payload payload)
    {
        if (this.IsNetworked)
        {
            base.Send(payload);
            return;
        }

        if (this.Game is null)
            this.ConnectedClient = new BlackClient(string.Empty, string.Empty);

        this.OnReceive(payload.AsBytes()!); // emulate a networked activity
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
            return;
        }

        this.BottomSheet!.Callback.ToState = null;
        this.BottomSheet!.Behavior.State = (int)VisibilityState.Collapsed;
        this.ConnectedClient ??= JsonSerializer.Deserialize(payload, SourceJsonGenerationContext.Default.IPlayerClient);
        if (this.ConnectedClient is null)
        {
            Logger.Warn("Unknown state something went wrong");
            return;
        }

        if (this.ConnectedClient.IsWhite)
        {
            this.WhitePlayerUsername!.Text = this.ConnectedClient.Username;
            var profilePicture = this.WhitePlayerProfilePicture!;
            this.ConnectedClient.TryLoadProfilePicture(Glide.With(this))?.Placeholder(profilePicture.Drawable!).Into(profilePicture);
        }

        else
        {
            this.BlackPlayerUsername!.Text = this.ConnectedClient.Username;
            var profilePicture = this.BlackPlayerProfilePicture!;
            this.ConnectedClient.TryLoadProfilePicture(Glide.With(this))?.Placeholder(profilePicture.Drawable!).Into(profilePicture);
        }

        this.Game = new ChessGame(this);
    }

    /// <remarks> Someone who is connected to us has sent us data. Overridden this method to handle this event. </remarks>
    /// <summary> Handles the payload sent by endpoint client </summary>
    /// <param name="endpoint"> The client who is sending the payload to us </param>
    /// <param name="payload"> The Payload containing all the data for us to handle the event </param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType != Payload.Type.Bytes) return;
        this.OnReceive(payload.AsBytes()!);
    }

    /// <summary> Called when someone has disconnected. Overridden this method to inform the user about the event. </summary>
    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        if (this.Game is null || !this.IsNetworked) return;

        Toast.MakeText(this, $"Error, {this.Client!.Username} disconnected", ToastLength.Short)?.Show();
        var description = $"{this.ConnectedClient.Username} disconnected, {this.Client.Username} wins by technicality";
        switch (this.Client.IsWhite) // Inverse because our client did not disconnect
        {
            case true: // our client is white and the connect client is black
                this.BottomSheet?.ShowGameOver(this.Game.BlackPlayer!, description);
                break;

            default:
                this.BottomSheet?.ShowGameOver(this.Game.WhitePlayer!, description);
                break;
        }
    }
}
