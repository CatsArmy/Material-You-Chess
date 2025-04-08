using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.Extensions;
using Chess.App.Nearby;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Player;
using Firebase.Auth;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Networked;

[Activity(Label = "@string/app_name", ScreenOrientation = ScreenOrientation.Portrait,
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public partial class NetworkedChessActivity : IChessActivity
{
    public Context? Context => this;

    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ExtendedFloatingActionButton? Home { get; set; }

    public BottomSheetCallback? Callback { get; set; }
    public ImageView? Indicator { get; set; }
    public ShapeableImageView? WinningPlayer { get; set; }
    public TextView? WinnerUsername { get; set; }
    public TextView? WinnerDescription { get; set; }

    public required ChessGame Game { get; set; }
    public required UserClient Client { get; set; }
    public required UserClient ConnectedClient { get; set; }

    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    protected override string AdvertisingName => this.CurrentUser.Uid;
    protected override Strategy Strategy => Strategy.P2pStar;

    private const string UserIsNull = "FirebaseAuth.Instance.CurrentUser is null somehow";
    private readonly FirebaseUser CurrentUser = FirebaseAuth.Instance.CurrentUser ?? throw new NullReferenceException(UserIsNull);

    public override void Finish() => base.Finish();

    public void EndGame(IPlayer winner, IPlayer loser)
    {
        this.ChessBottomSheet?.Show(winner, loser);
        this.BottomSheet!.State = BottomSheetBehavior.StateExpanded;
        this.Game.Cleanup();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (!this.MaterialYouThemePreference())
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity); //Set our view
        base.SetResult(Result.FirstUser);
        this.PromotionDialogs = (new(this), new(this));

        this.StandardBottomSheet = base.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet);
        this.BottomSheetLayout = base.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet);
        this.MatchmakingLayout = base.FindViewById<ConstraintLayout>(Resource.Id.matchmaking);
        this.GameOverLayout = base.FindViewById<ConstraintLayout>(Resource.Id.game_over);

        this.Indicator = base.FindViewById<ImageView>(Resource.Id.winningPlayerIndicator);
        this.WinningPlayer = base.FindViewById<ShapeableImageView>(Resource.Id.winningPlayer);
        this.WinnerUsername = base.FindViewById<TextView>(Resource.Id.winningPlayerUsername);
        this.WinnerDescription = base.FindViewById<TextView>(Resource.Id.winnerDescription);

        this.WhitePlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.Home = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.home);

        this.BottomSheet = BottomSheetBehavior.From(this.BottomSheetLayout!);
        this.Callback = new BottomSheetCallback(this);
        this.ChessBottomSheet = new(this);
        this.CreateBottomSheet();
        this.Show();
    }

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        var firebaseUserClient = this.CurrentUser;
        if (this.State == State.Advertising)
        {
            Logger.Verbose("Client is white player");
            var client = new WhitePlayerClient(firebaseUserClient!);
            client.LoadProfilePicture(Glide.With(this)).Into(this.WhitePlayerProfilePicture!);
            this.WhitePlayerUsername!.Text = client!.Username;
            this.Client = client;
        }

        if (this.State == State.Discovering)
        {
            Logger.Verbose("Client is black player");
            var client = new BlackPlayerClient(firebaseUserClient!);
            client.LoadProfilePicture(Glide.With(this)).Into(this.BlackPlayerProfilePicture!);
            this.BlackPlayerUsername!.Text = client.Username;
            this.Client = client;
        }

        //Init handshake
        this.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(this.Client, SourceJsonGenerationContext.Default.FirebaseUserClient)));
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

    /// <summary> Allows <see cref="IChessActivity"/> access to <see cref="ConnectionsActivity.Send(Payload)"/>
    /// by both implementing <see cref="IChessActivity.Send(Payload)"/> method and overriding the 
    /// <see cref="ConnectionsActivity.Send(Payload)"/> method </summary>
    public override void Send(Payload payload) => base.Send(payload);

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

        this.Hide();
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
