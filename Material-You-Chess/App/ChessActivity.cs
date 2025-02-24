using System.Text;
using System.Text.Json;
using Android.Content.PM;
using Android.Gms.Nearby.Connection;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.App.Networked;
using Chess.Game;
using Chess.Game.Moves;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
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

        this.FindViewById<TextView>(Resource.Id.p1MainUsername)!.Text = "Player 1";
        this.FindViewById<TextView>(Resource.Id.p2MainUsername)!.Text = "Player 2";

        var board = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        this.game = new(this, "Player 1", "Player 2", board!, new(this), new(this), true, this.Send);
    }

    private void Send(Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {
            var json = Encoding.UTF8.GetString(payload.AsBytes()!);
            Logger.Error(json);
            //var DOM = JsonDocument.Parse(json)!;
            //string typeDiscriminator = DOM.RootElement.GetProperty("$type").GetString()!;
            //var JsonTypeInfo = SourceJsonGenerationContext.Default.GetTypeInfo(Type.GetType(typeDiscriminator)!);
            Move? move = JsonSerializer.Deserialize<Move>(json, SourceJsonGenerationContext.Default.Options);
            move!.OriginPiece.Move(move, this.game!);
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
            this.game?.RedrawGame();
        }
    }
}
