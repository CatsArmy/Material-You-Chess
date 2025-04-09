using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common;
using Chess.App.Nearby;
using Chess.App.Networked;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Player;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Portrait)]
public class ChessActivity : AppCompatActivity, IChessActivity
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
    public ChessBottomSheet? ChessBottomSheet { get; set; }
    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }
    public ConstraintLayout? MatchmakingLayout { get; set; }
    public ConstraintLayout? GameOverLayout { get; set; }

    public ImageView? Indicator { get; set; }
    public ShapeableImageView? WinningPlayer { get; set; }
    public TextView? WinnerUsername { get; set; }
    public TextView? WinnerDescription { get; set; }

    public required ChessGame Game { get; set; }
    public required UserClient Client { get; set; } = new("White Player");
    public required UserClient ConnectedClient { get; set; } = new("Black Player");

    public override void Finish() => base.Finish();

    public void EndGame(IPlayer winner, IPlayer loser)
    {
        this.ChessBottomSheet?.Show(winner, loser);
        this.BottomSheet!.State = BottomSheetBehavior.StateExpanded;
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity);
        base.SetResult(Result.FirstUser);

        this.StandardBottomSheet = base.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet);
        this.BottomSheetLayout = base.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet);
        this.MatchmakingLayout = base.FindViewById<ConstraintLayout>(Resource.Id.matchmaking);
        this.GameOverLayout = base.FindViewById<ConstraintLayout>(Resource.Id.game_over);

        this.BottomSheet = BottomSheetBehavior.From(this.BottomSheetLayout!);

        this.Indicator = base.FindViewById<ImageView>(Resource.Id.winningPlayerIndicator);
        this.WinningPlayer = base.FindViewById<ShapeableImageView>(Resource.Id.winningPlayer);
        this.WinnerUsername = base.FindViewById<TextView>(Resource.Id.winningPlayerUsername);
        this.WinnerDescription = base.FindViewById<TextView>(Resource.Id.winnerDescription);

        this.WhitePlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.Home = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.home);

        this.WhitePlayerUsername!.Text = this.Client!.Username;
        this.BlackPlayerUsername!.Text = this.ConnectedClient!.Username;

        this.Callback = new BottomSheetCallback(this);
        this.ChessBottomSheet = new(this);
        //for (int i = Resource.Id.gmb__A1; i <= Resource.Id.gmp__wRook2; i++)
        //{
        //    base.FindViewById(i)!.Click += (sender, args) => { this.Game.OnClick(sender, args); };
        //} bug cause: unknown; view.isattachedtowindow, view.handler is null
        this.Game = new(this);
    }

    public void Send(Payload payload) //Emulate a networked chess activity
    {
        var move = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.Move);
        this.Game.PlayMove(move!, false);
    }

    protected override void OnDestroy()
    {
        Logger.Debug($"{nameof(OnDestroy)}");
        base.OnDestroy();
    }
}

public class OnBackPressedCallback(bool enabled) : AndroidX.Activity.OnBackPressedCallback(enabled)
{
    public override void HandleOnBackPressed()
    {

    }
}