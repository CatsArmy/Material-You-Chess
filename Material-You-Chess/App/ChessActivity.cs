using System.Text;
using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Networked;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Moves;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = ScreenOrientation.Locked)]
public class ChessActivity : AppCompatActivity, IChessActivity
{
    public ChessGame? Game { get; set; }
    public Context? Context { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public string? WhitePlayerName => "Player 1";
    public string? BlackPlayerName => "Player 2";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"), out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        this.Context = this;

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.WhitePlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.BlackPlayerProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.WhitePlayerUsername = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.BlackPlayerUsername = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));
        this.WhitePlayerUsername!.Text = this.WhitePlayerName;
        this.BlackPlayerUsername!.Text = this.BlackPlayerName;

        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.Game = new(this);
    }

    public void Send(Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var json = Encoding.UTF8.GetString(payload.AsBytes()!);
            Logger.Error(json);
            Move? move = JsonSerializer.Deserialize(json, SourceJsonGenerationContext.Default.Move);
        }
    }
}
