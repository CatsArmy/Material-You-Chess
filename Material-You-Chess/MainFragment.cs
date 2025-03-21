using Android.Content;
using Android.Views;
using Chess.App;
using Chess.App.Common;
using Chess.App.Networked;
using Google.Android.Material.Button;

namespace Chess;

public class MainFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    public MaterialButtonToggleGroup? GameModeSelector { get; private set; }
    public Button? Online { get; private set; }
    public Button? Local { get; private set; }
    public Button? Start { get; private set; }
    private PermissionsRequester? permissionsRequester;
    public bool IsLoggedIn
    {
        get;
        set
        {
            field = value;
            if (this.Online is null)
                return;

            this.Online!.Enabled = value;
        }
    } = false;

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.Start = view.FindViewById<Button>(Resource.Id.btnStartGame);
        this.Start!.Click += this.StartGame;

        this.Online = view.FindViewById<Button>(Resource.Id.btnOnline);
        this.GameModeSelector = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.GameModeSelector);
        this.Local = view.FindViewById<Button>(Resource.Id.btnLocal);
        this.GameModeSelector!.Check(this.Local!.Id);
        this.Online!.Enabled = this.IsLoggedIn;
        this.permissionsRequester = new(this.Activity!);
    }

    private void StartGame(object? sender, EventArgs e)
    {
        if (this.GameModeSelector!.CheckedButtonId == Resource.Id.btnOnline)
        {
            this.StartOnlineGame();
            return;
        }

        base.StartActivity(new Intent(this.Activity!,
            typeof(ChessActivity))
            .PutExtra(nameof(Main_Activity.MaterialYouThemePreference),
            $"{Main_Activity.Instance?.MaterialYouThemePreference}"));
    }

    private void StartOnlineGame()
    {
        if (!this.permissionsRequester!.HasNearbyAccess())
        {
            this.permissionsRequester.RequestNearbyConnectionsAccess(() => base.StartActivity(new Intent(this.Activity!,
                typeof(NetworkedChessActivity))
                .PutExtra(nameof(Main_Activity.MaterialYouThemePreference),
                $"{Main_Activity.Instance?.MaterialYouThemePreference}")));
            return;
        }

        base.StartActivity(new Intent(this.Activity!,
            typeof(NetworkedChessActivity))
            .PutExtra(nameof(Main_Activity.MaterialYouThemePreference),
            $"{Main_Activity.Instance?.MaterialYouThemePreference}"));
    }
}
