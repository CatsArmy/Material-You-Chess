using Android.Content.PM;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Game;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
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

        this.p1MainUsername = FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.p2MainUsername = FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.p1MainUsername!.Text = "Player 1";
        this.p2MainUsername!.Text = "Player 2";

        //TODO Add Castling difficulty Medium
        //TODO Add Promotion difficulty Easy+ / Medium-
        //TODO More?

        //FIX why are the captured pieces not captured??
        var board = FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.game = new ChessGame(board!, null, null);
    }
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
