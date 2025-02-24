using System.Text;
using System.Text.Json;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.App.Networked;
using Chess.Dialogs;
using Chess.Game;
using Chess.Game.Moves;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity, IChessActivity
{
    public ChessGame? Game { get; set; }
    public Context? Context { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? Player1ShapeableImageView { get; set; }
    public ShapeableImageView? Player2ShapeableImageView { get; set; }
    public TextView? Profile1Username { get; set; }
    public TextView? Profile2Username { get; set; }

    public string? Player1Name => "Player 1";
    public string? Player2Name => "Player 2";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra("MaterialYouThemePreference"), out var MaterialYouThemePreference);
        if (hasValue && !MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        this.Context = this;

        // Permission request logic
        _ = new PermissionsRequester(this);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.Player1ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.Player2ShapeableImageView = this.FindViewById<ShapeableImageView>(Resource.Id.p2MainProfileImageView);
        this.Profile1Username = this.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.Profile2Username = this.FindViewById<TextView>(Resource.Id.p2MainUsername);
        this.PromotionDialogs = (new(this), new(this));
        this.Profile1Username!.Text = this.Player1Name;
        this.Profile2Username!.Text = this.Player2Name;

        this.BoardLayout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.Game = new(this, true);
    }

    public void Send(Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var json = Encoding.UTF8.GetString(payload.AsBytes()!);
            Logger.Error(json);
            Move? move = JsonSerializer.Deserialize<Move>(json, SourceJsonGenerationContext.Default.Move);
            move!.Origin.Move(move, this.Game!);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }


    public override void Finish()
    {
        ChessGame.Instance = null;
        base.Finish();
    }

    public override ScreenOrientation RequestedOrientation
    {
        get => base.RequestedOrientation; set
        {
            base.RequestedOrientation = value;
            this.Game?.RedrawGame();
        }
    }
}
