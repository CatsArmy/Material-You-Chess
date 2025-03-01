using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Graphics;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Nearby;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Moves;
using Chess.Game.Player;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked)]
public class NetworkedChessActivity : ConnectionsActivity, IChessActivity
{
    /// <summary>
    /// <see langword="if" /> <see cref="isConnectionInitiator" /> <see langword="is" /> <see langword="true" />:
    /// Player1 <see langword="is"/> <see cref="White"/>
    /// <br />
    /// <see langword="if" /> <see cref="isConnectionInitiator" /> <see langword="is" /> <see langword="false" />:
    /// Player2 <see langword="is"/> <see cref="Black"/>
    /// </summary>

    private bool isConnectionInitiator = false;
    private LobbyWaitingRoomBottomSheet? LobbyWaitingRoom;

    public ChessGame? Game { get; set; }
    public Context? Context { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? Player1ShapeableImageView { get; set; }
    public ShapeableImageView? Player2ShapeableImageView { get; set; }
    public TextView? Profile1Username { get; set; }
    public TextView? Profile2Username { get; set; }

    public string Player1Name { get; private set; } = string.Empty;
    public string Player2Name { get; private set; } = string.Empty;

    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string Name => FirebaseAuth.Instance.CurrentUser!.DisplayName!;
    protected override Strategy Strategy => Strategy.P2pStar;

    protected State State
    {
        get; set
        {
            if (field == value)
            {
                Logger.Warn($"State set to {field} but already in that state");
                field = value;
                return;
            }
            field = value;
            switch (field)
            {
                case State.Searching:
                    this.DisconnectFromAllEndpoints();
                    this.StartAdvertising();
                    break;
                case State.Connected:
                    //Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
                    this.StopDiscovering();
                    this.StopAdvertising();
                    break;
                case State.Connecting:
                    //this.StopAdvertising();
                    break;
                case State.Unknown:
                    this.StopAllEndpoints();
                    break;
            }
        }
    } = State.Unknown;

    public void IsHost_CheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        this.isConnectionInitiator = e.IsChecked;
        switch (e.IsChecked)
        {
            case true:
                this.StopAdvertising();
                this.StartDiscovering();
                break;

            case false:
                this.StartAdvertising();
                this.StopDiscovering();
                break;
        }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"), out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.Player1ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.Player2ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);

        this.Profile1Username = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.Profile2Username = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));
        this.BoardLayout = this.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
    }

    protected override void OnStart()
    {
        base.OnStart();
        this.State = State.Searching;

        //set game view to waiting for opponent 
        Logger.Debug($"::new {nameof(LobbyWaitingRoomBottomSheet)}()::");
        this.LobbyWaitingRoom = new LobbyWaitingRoomBottomSheet(this);
        Logger.Debug($"::show {nameof(LobbyWaitingRoomBottomSheet)}()::");
        this.LobbyWaitingRoom.Show(this.SupportFragmentManager, "Lobby Waiting Room");
        Logger.Debug($"::showing {nameof(LobbyWaitingRoomBottomSheet)}()::");
    }

    protected override void OnStop()
    {
        // After our Activity stops, we disconnect from Nearby Connections.
        if (this.State == State.Connected)
            return;

        this.State = State.Unknown;
        base.OnStop();
    }

    public override void Finish()
    {
        this.State = State.Unknown;
        base.Finish();
    }

    public override void Send(Payload payload) => base.Send(payload);

    protected override async Task OnEndpointDiscoveredAsync(EndPoint endpoint)
    {
        //We found an advertiser!
        this.StopDiscovering();
        await Task.Delay(10);
        if (base.IsConnecting)
        {
            return;
        }
        this.isConnectionInitiator = true;
        this.ConnectToEndpoint(endpoint);
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo)
    {
        // A connection to another device has been initiated! We'll use the auth token, which is the
        // same on both devices, to pick a color to use when we're connected. This way, users can
        // visually see which device they connected with.
        // We accept the connection immediately.

        if (this.State == State.Connecting)
        {
            return;
        }
        this.State = State.Connecting;
        this.isConnectionInitiator = false;
        this.AcceptConnection(endpoint);
    }

    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Found opponent, {endpoint.Name}", ToastLength.Short)?.Show();
        this.State = State.Connected;
        var stream = new MemoryStream();
        switch (this.isConnectionInitiator)
        {
            case true:
                Logger.Error("isConnection Initiator true");
                this.Profile1Username!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
                };
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    //var load =
                    //Glide.With(this).Load(FirebaseStorage.Instance.Reference
                    //.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png"))
                    //.Error(Resource.Drawable.outline_account_circle_24)
                    //.Into(this.Player1ShapeableImageView!);
                }
                this.Profile2Username!.Text = endpoint.Name;
                this.Player2Name = endpoint.Name;
                this.Player1Name = FirebaseAuth.Instance?.CurrentUser?.DisplayName!;
                this.Game = new ChessGame(this, true);
                if (this.Player1ShapeableImageView == null)
                    return;

                //if (!(this.Player1ShapeableImageView.Drawable as BitmapDrawable)!.Bitmap!.Compress(Bitmap.CompressFormat.Png!, 100, stream))
                //    return;
                break;

            case false:
                Logger.Error("isConnection Initiator false");
                this.Player2Name = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName!,
                };

                this.Profile2Username!.Text = this.Player2Name;
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    //var load = 
                    //Glide.With(this).Load(FirebaseStorage.Instance.Reference
                    //.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png"))
                    //.Error(Resource.Drawable.outline_account_circle_24)
                    //.Into(this.Player2ShapeableImageView!);
                }
                this.Player1Name = endpoint.Name;
                this.Profile1Username!.Text = this.Player1Name;

                this.Game = new ChessGame(this, false);
                if (this.Player2ShapeableImageView == null)
                    return;

                //if (!(this.Player2ShapeableImageView.Drawable as BitmapDrawable)!.Bitmap!.Compress(Bitmap.CompressFormat.Png!, 100, stream))
                //    return;
                break;
        }

        //this.Send(Payload.FromStream(stream));
    }

    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Error, {endpoint.Name} disconnected", ToastLength.Short)?.Show();
        this.State = State.Unknown;
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {
        this.State = State.Unknown;
        this.State = State.Searching;
        this.isConnectionInitiator = false;
        this.StartDiscovering();
    }

    /// <summary>
    /// Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client
    /// </summary>
    /// <param name="endpoint">The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload">The <see cref="Payload"/> containing all the data for us to handle the event</param>
    [SuppressMessage("Trimming",
        "IL2057:Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.")]
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var json = Encoding.UTF8.GetString(payload.AsBytes()!);
            Move? move = JsonSerializer.Deserialize(json, SourceJsonGenerationContext.Default.Move);
            move!.Origin.Move(move, this.Game!);
        }

        if (payload.PayloadType == Payload.Type.Stream)
        {
            var stream = new MemoryStream();
            payload.AsStream()!.AsInputStream().CopyToAsync(stream).Wait();

            var pfp = ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(stream.ToArray()));
            switch (this.isConnectionInitiator)
            {
                case true:
                    Glide.With(this).Load(pfp).Into(this.Player2ShapeableImageView!);
                    break;

                case false:
                    Glide.With(this).Load(pfp).Into(this.Player1ShapeableImageView!);
                    break;
            }

        }
    }

    protected override string[] GetRequiredPermissions()
    {
        var perms = base.GetRequiredPermissions().ToList();
        var newPerms = new List<string>();
        newPerms.AddRange(perms);
        return [.. newPerms];
    }
}
