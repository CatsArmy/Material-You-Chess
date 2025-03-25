using Android.Content;
using Android.Content.PM;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.App.Common.Extensions;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Player;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked)]
public class ChessActivity : AppCompatActivity, IChessActivity
{
    public Context? Context => this;

    public required ChessGame Game { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }

    public required UserClient Client { get; set; } = new("White Player");
    public required UserClient ConnectedClient { get; set; } = new("Black Player");

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        if (!this.MaterialYouThemePreference())
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.WhitePlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));

        this.WhitePlayerUsername!.Text = this.Client!.Username;
        this.BlackPlayerUsername!.Text = this.ConnectedClient!.Username;

        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.Game = new(this);
    }
}
