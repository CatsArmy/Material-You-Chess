using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.Game;
using Chess.Game.Common;
using Firebase.Auth;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
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
    protected const string UserIsNull = "FirebaseAuth.Instance.CurrentUser is null somehow";
    protected readonly FirebaseUser CurrentUser = FirebaseAuth.Instance.CurrentUser ?? throw new NullReferenceException(UserIsNull);
    protected override Strategy Strategy => Strategy.P2pStar;
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName => this.CurrentUser.Uid;

    public Context? Context => this;
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }

    public required ChessGame Game { get; set; }
    public required UserClient Client { get; set; }
    public required UserClient ConnectedClient { get; set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity);
        base.SetResult(Result.FirstUser);
        IChessActivity.Instance = this;
        this.PromotionDialogs = (new(this), new(this));

        this.WhitePlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.BottomSheet = new(this);
    }

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
        if (this.Game != null)
        {
            Toast.MakeText(this, $"Error, {this.Client!.Username} disconnected", ToastLength.Short)?.Show();
            this.SetResult(Result.Canceled);
            this.Finish();
        }
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
        this.BottomSheet!.Behavior.State = (int)ChessBottomSheet.VisibilityState.Collapsed;
        switch (JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.FirebaseUserClient))
        {
            case WhitePlayerClient whiteClient:
                this.ConnectedClient = whiteClient;
                this.WhitePlayerUsername!.Text = whiteClient.Username;
                whiteClient.LoadProfilePicture(Glide.With(this))
                    .Placeholder(this.WhitePlayerProfilePicture!.Drawable!)
                    .Into(this.WhitePlayerProfilePicture!);
                break;

            case BlackPlayerClient blackClient:
                this.ConnectedClient = blackClient;
                this.BlackPlayerUsername!.Text = blackClient.Username;
                blackClient.LoadProfilePicture(Glide.With(this))
                    .Placeholder(this.BlackPlayerProfilePicture!.Drawable!)
                    .Into(this.BlackPlayerProfilePicture!);
                break;

            default:
                Logger.Warn("Unknown state something went wrong");
                return;
        }

        this.Game = new ChessGame(this);
    }

    protected override void OnDestroy()
    {
        IChessActivity.Instance = null;
        Logger.Debug($"{nameof(OnDestroy)}");
        base.OnDestroy();
    }
}
