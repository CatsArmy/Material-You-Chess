using System.Text;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Runtime;
using AndroidX.ConstraintLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Nearby;
using Chess.Game;
using Chess.Game.Moves;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;
using Newtonsoft.Json;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class NetworkedChessActivity : ConnectionsActivity
{
    protected override Strategy Strategy => Strategy.P2pStar;
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string Name => FirebaseAuth.Instance.CurrentUser!.DisplayName!;

    private State State
    {
        get => field;
        set
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
                    this.StartDiscovering();
                    this.StartAdvertising();
                    break;
                case State.Connected:
                    this.StopDiscovering();
                    this.StopAdvertising();
                    break;
                case State.Unknown:
                    this.StopAllEndpoints();
                    break;
            }
        }
    } = State.Unknown;

    private ShapeableImageView? p1MainProfileImageView;
    private ShapeableImageView? p2MainProfileImageView;
    private TextView? p1MainUsername;
    private TextView? p2MainUsername;
    private ChessGame? game;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"), out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        // Permission request logic
        _ = new PermissionsRequester(this);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.p1MainProfileImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
        {
            var load = Glide.With(this).Load(FirebaseStorage.Instance.Reference
                .Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png")).Error(Resource.Drawable.outline_account_circle_24);
            new Thread((requestBuilder) => { (requestBuilder as RequestBuilder)?.Into(this.p1MainProfileImageView!); }).Start(load);
        }

        this.p2MainProfileImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);

        this.p1MainUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.p2MainUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);

        this.p1MainUsername!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
        {
            true => "Player",
            false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
        };
        var board = this.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.game = new ChessGame(board!, null, this.Send);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnStart()
    {
        base.OnStart();
        this.State = State.Searching;
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

    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        // We found an advertiser!
        this.StopDiscovering();
        Thread.Sleep(10);
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
        this.AcceptConnection(endpoint);
    }

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_connected, {endpoint.Name}", ToastLength.Short)?.Show();
        this.State = State.Connected;
    }


    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_disconnected, {endpoint.Name}", ToastLength.Short)?.Show();
        this.State = State.Searching;
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {
        this.State = State.Searching;
        this.StartDiscovering();
    }

    /// <summary>
    /// Handles the <paramref name="payload"/> sent by <paramref name="endpoint"/> client
    /// </summary>
    /// <param name="endpoint">The client who is sending the <paramref name="payload"/> to us </param>
    /// <param name="payload">The <see cref="Payload"/> containing all the data for us to handle the event</param>
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var move = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(payload.AsBytes()!), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            }) as IMove;
            this.game?.OnMove(move!);
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

