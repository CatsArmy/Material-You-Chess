using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Graphics;
using Android.Graphics.Drawables;
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

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", ScreenOrientation = ScreenOrientation.Locked)]
public class NetworkedChessActivity : ConnectionsActivity
{
    protected override Strategy Strategy => Strategy.P2pStar;
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string Name => FirebaseAuth.Instance.CurrentUser!.DisplayName!;

    /// <summary>
    /// if true => white player / p1
    /// if false => black player / p2
    /// </summary>
    private bool isConnectionInitiator = false;
    private ShapeableImageView? p1MainProfileImageView;
    private ShapeableImageView? p2MainProfileImageView;
    private TextView? p1MainUsername;
    private TextView? p2MainUsername;
    private ChessGame? game;

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

    protected override async Task OnAdvertisingStartedAsync()
    {
        await base.OnAdvertisingStartedAsync();
        await Task.Delay(TimeSpan.FromSeconds(3));
        this.StartDiscovering();
    }

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


        this.p2MainProfileImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);

        this.p1MainUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.p2MainUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        //set game view to waiting for opponent 
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

    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "<Pending>")]
    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Found opponent, {endpoint.Name}", ToastLength.Short)?.Show();
        this.State = State.Connected;
        var board = this.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        var stream = new MemoryStream();
        switch (this.isConnectionInitiator)
        {
            case true:
                Logger.Error("isConnection Initiator true");
                this.p1MainUsername!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
                };
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    var load = Glide.With(this).Load(FirebaseStorage.Instance.Reference
                        .Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png"))
                        .Error(Resource.Drawable.outline_account_circle_24)
                        .Into(this.p1MainProfileImageView!);
                }
                this.p2MainUsername!.Text = endpoint.Name;
                this.game = new ChessGame(this, FirebaseAuth.Instance?.CurrentUser?.DisplayName!, endpoint.Name, board!,
            new(this), new(this), true, this.Send);
                if (this.p1MainProfileImageView == null)
                    return;

                if (!(this.p1MainProfileImageView.Drawable as BitmapDrawable)!.Bitmap!.Compress(Bitmap.CompressFormat.Png!, 100, stream))
                    return;
                break;

            case false:
                Logger.Error("isConnection Initiator false");
                this.p2MainUsername!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
                {
                    true => "Player",
                    false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
                };
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    var load = Glide.With(this).Load(FirebaseStorage.Instance.Reference
                        .Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png"))
                        .Error(Resource.Drawable.outline_account_circle_24)
                        .Into(this.p2MainProfileImageView!);
                }
                this.p1MainUsername!.Text = endpoint.Name;
                this.game = new ChessGame(this, FirebaseAuth.Instance?.CurrentUser?.DisplayName!, endpoint.Name, board!,
            new(this), new(this), false, this.Send);
                if (this.p2MainProfileImageView == null)
                    return;

                if (!(this.p2MainProfileImageView.Drawable as BitmapDrawable)!.Bitmap!.Compress(Bitmap.CompressFormat.Png!, 100, stream))
                    return;
                break;
        }

        this.Send(Payload.FromStream(stream));
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
        //this.isConnectionInitiator = null;
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
            var json = Encoding.UTF8.GetString(payload.AsBytes()!);
            IMove move;
            if (json.Contains(nameof(PromoteBishop)))
                move = JsonSerializer.Deserialize<PromoteBishop>(json);
            else if (json.Contains(nameof(PromoteBishopAndCapture)))
                move = JsonSerializer.Deserialize<PromoteBishopAndCapture>(json);
            else if (json.Contains(nameof(PromoteQueen)))
                move = JsonSerializer.Deserialize<PromoteQueen>(json);
            else if (json.Contains(nameof(PromoteQueenAndCapture)))
                move = JsonSerializer.Deserialize<PromoteQueenAndCapture>(json);
            else if (json.Contains(nameof(PromoteRook)))
                move = JsonSerializer.Deserialize<PromoteRook>(json);
            else if (json.Contains(nameof(PromoteRookAndCapture)))
                move = JsonSerializer.Deserialize<PromoteRookAndCapture>(json);
            else if (json.Contains(nameof(PromoteKnight)))
                move = JsonSerializer.Deserialize<PromoteKnight>(json);
            else if (json.Contains(nameof(PromoteKnightAndCapture)))
                move = JsonSerializer.Deserialize<PromoteKnightAndCapture>(json);
            else if (json.Contains(nameof(Capture)))
                move = JsonSerializer.Deserialize<Capture>(json);
            else if (json.Contains(nameof(DoubleMove)))
                move = JsonSerializer.Deserialize<DoubleMove>(json);
            else if (json.Contains(nameof(EnPassant)))
                move = JsonSerializer.Deserialize<EnPassant>(json);
            //else if (json.Contains(nameof(KingSideCastle)))
            //{ /*move = JsonSerializer.Deserialize<KingSideCastle>(json);*/}
            //else if (json.Contains(nameof(QueenSideCastle)))
            //{  /*move = JsonSerializer.Deserialize<QueenSideCastle>(json);*/}
            else
                return;
            Logger.Error(json);

            this.game?.OnMove(move!);
        }

        if (payload.PayloadType == Payload.Type.Stream)
        {
            var stream = new MemoryStream();
            payload.AsStream()!.AsInputStream().CopyToAsync(stream).Wait();

            var pfp = ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(stream.ToArray()));
            switch (this.isConnectionInitiator)
            {
                case true:
                    Glide.With(this).Load(pfp).Into(this.p2MainProfileImageView!);
                    break;

                case false:
                    Glide.With(this).Load(pfp).Into(this.p1MainProfileImageView!);
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

