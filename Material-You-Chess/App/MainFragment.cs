using Android.Content;
using Android.Views;
using Chess.App.Networked;
using Firebase.Auth;
using Google.Android.Material.Button;

namespace Chess.App;

public class MainFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    private FirebaseAuth? Auth;
    public MaterialButtonToggleGroup? GameModeSelector { get; private set; }
    public Button? Online { get; private set; }
    public Button? Local { get; private set; }
    public Button? Start { get; private set; }

    public bool IsLoggedIn
    {
        get; set
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
        this.Auth = (this.Activity as MainActivity)!.Auth;
        this.IsLoggedIn = this.Auth!.CurrentUser is not null;
        this.Auth!.AuthState += this.OnAuthState;
    }

    public override void OnDestroy()
    {
        this.Auth!.AuthState -= this.OnAuthState;
        base.OnDestroy();
    }

    public void OnAuthState(object? sender, FirebaseAuth.AuthStateEventArgs args)
    {
        if (this.Online is null)
            return;

        this.Online.Enabled = args.Auth.CurrentUser is not null;
    }

    private void StartGame(object? sender, EventArgs e)
    {
        base.StartActivity(new Intent(this.Activity!, (this.GameModeSelector!.CheckedButtonId == Resource.Id.btnOnline) switch
        {
            false => typeof(ChessActivity),
            true => typeof(NetworkedChessActivity),
        }));
    }
}
