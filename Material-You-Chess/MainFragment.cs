using Android.Content;
using Android.Views;
using Chess.App;
using Chess.App.Networked;
using Google.Android.Material.Button;

namespace Chess;

public class MainFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    private MaterialButtonToggleGroup? GameModeSelector;
    private Button? Online;
    private Button? Local;
    private Button? Start;

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.Start = view.FindViewById<Button>(Resource.Id.btnStartGame);
        this.Start!.Click += this.StartGame;

        this.Online = view.FindViewById<Button>(Resource.Id.btnOnline);
        this.GameModeSelector = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.GameModeSelector);
        this.Local = view.FindViewById<Button>(Resource.Id.btnLocal);
        this.GameModeSelector!.Check(this.Local!.Id);

        //this.Start!.Text = FirebaseAuth.Instance.CurrentUser?.DisplayName;
    }

    private void StartGame(object? sender, EventArgs e)
    {
        Intent intent = (this.GameModeSelector!.CheckedButtonId switch
        {
            Resource.Id.btnOnline => new Intent(this.Activity!, typeof(NetworkedChessActivity)),
            _ => new Intent(this.Activity!, typeof(ChessActivity))
        }).PutExtra(nameof(Main.MaterialYouThemePreference), $"{Main.Instance?.MaterialYouThemePreference}");

        base.StartActivity(intent);
    }
}
