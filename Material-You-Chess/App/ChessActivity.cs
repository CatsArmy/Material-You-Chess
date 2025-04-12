using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.App.Networked.Nearby;
using Chess.Game;
using Chess.Game.Common;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked,
    EnableOnBackInvokedCallback = true
)]
public class ChessActivity : AppCompatActivity, IChessActivity
{
    public Context? Context => this;
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ChessBottomSheet? BottomSheet { get; set; }
    public required ChessGame Game { get; set; }
    public required UserClient Client { get; set; } = new("White Player");
    public required UserClient ConnectedClient { get; set; } = new("Black Player");

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        base.SetContentView(Resource.Layout.chess_activity);
        base.SetResult(Result.FirstUser);
        IChessActivity.Instance = this;
        this.PromotionDialogs = (new(this), new(this));
        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.WhitePlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.WhitePlayerUsername!.Text = this.Client!.Username;
        this.BlackPlayerUsername!.Text = this.ConnectedClient!.Username;
        this.BottomSheet = new(this);
        this.Game = new(this);
    }

    public void Send(Payload payload) //Emulate a networked chess activity
    {
        var move = JsonSerializer.Deserialize(payload.AsBytes()!, SourceJsonGenerationContext.Default.Move);
        this.Game.PlayMove(move!, false);
    }
}
