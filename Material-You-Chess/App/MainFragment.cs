using Android.Content;
using Android.Views;
using Chess.App.Common.Extensions;
using Chess.App.Common.Permissions;
using Chess.App.Networked.Nearby;
using Firebase.Auth;
using Google.Android.Material.Button;

namespace Chess.App;

public class MainFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    private FirebaseAuth? Auth;
    private NearbyConnections? PermissionManager;
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

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        //this.PermissionManager = this.RegisterNearbyPermissionsManager(this.HandlePermissionResult);
    }

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

    /// <param name="isGranted"> <paramref name="isGranted"/> are all of the requested permissions granted </param>
    private void HandlePermissionResult(bool isGranted)
    {
        //if (!isGranted)
        //{
        //
        //}
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
        //when missing perms
        //this.PermissionManager?.RequestAccess();
        base.StartActivity(new Intent(this.Activity!, (this.GameModeSelector!.CheckedButtonId == Resource.Id.btnOnline) switch
        {
            false => typeof(ChessActivity),
            true => typeof(NetworkedChessActivity),
        }));
    }
}
