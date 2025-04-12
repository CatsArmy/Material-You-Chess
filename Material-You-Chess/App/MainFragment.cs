using Android.Content;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common.ActivityResult;
using Chess.App.Common.Extensions;
using Chess.App.Common.Permissions;
using Chess.App.Networked.Nearby;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Java.Util;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess.App;

public class MainFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    public MaterialButtonToggleGroup? GameModeSelector { get; private set; }
    public MaterialButton? Online { get; private set; }
    public MaterialButton? Local { get; private set; }
    public ExtendedFloatingActionButton? Start { get; private set; }

    public bool IsLoggedIn
    {
        get; set
        {
            field = value;
            if (this.Online is null) return;
            this.Online!.Enabled = value;
        }
    } = false;

    private bool ClickedStart = false;
    private FirebaseAuth? Auth;
    private NearbyConnections? PermissionManager;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.PermissionManager = new(this.RegisterForActivityResult(new RequestMultiplePermissions(), new ActivityResultCallback<IMap>(permissions => this.HandlePermissionResult(!permissions!.Values().Cast<bool>().ToArray().Contains(false)))), this.Activity);
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.Start = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.btnStartGame);
        this.Start!.Click += this.StartGame;

        this.Online = view.FindViewById<MaterialButton>(Resource.Id.btnOnline);
        this.GameModeSelector = view.FindViewById<MaterialButtonToggleGroup>(Resource.Id.GameModeSelector);
        this.Local = view.FindViewById<MaterialButton>(Resource.Id.btnLocal);
        this.GameModeSelector!.Check(this.Local!.Id);
        this.Online!.Enabled = this.IsLoggedIn;
        this.Online!.Click += this.OnCheckOnline;
        this.Local!.Click += this.OnCheckLocal;
        this.Auth = (this.Activity as MainActivity)!.Auth;
        this.IsLoggedIn = this.Auth!.CurrentUser is not null;
        this.Auth!.AuthState += this.OnAuthState;
    }

    private void OnCheckLocal(object? sender, EventArgs e)
    {
        this.Start!.SetIconResource(Resource.Drawable.group);
        this.Start!.Text = "Play";
    }

    private void OnCheckOnline(object? sender, EventArgs e)
    {
        this.PermissionManager?.RequestAccess();
    }

    /// <summary>
    /// Updates the ui based on the permission that was might have been granted
    /// also starts the networked chess activity if the permission requester was the onclick start</summary>
    /// <param name="isGranted"> <paramref name="isGranted"/> are all of the requested permissions granted </param>
    public void HandlePermissionResult(bool isGranted)
    {
        if (!isGranted)
        {
            this.Online!.SetIconResource(Resource.Drawable.nearby_off);
            this.Start!.SetIconResource(Resource.Drawable.nearby_error);
            this.Start!.Text = "Missing Permissions";
            return;
        }
        this.Online!.SetIconResource(Resource.Drawable.nearby);
        this.Start!.SetIconResource(Resource.Drawable.nearby);
        this.Start!.Text = "Play";
        if (this.ClickedStart)
        {
            this.ClickedStart = false;
            Thread.Sleep(TimeSpan.FromSeconds(0.4)); // add a slight delay to let the user slightly see the change in ui state
            base.StartActivity(new Intent(this.Activity!, typeof(NetworkedChessActivity)));
        }
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
        if ((this.GameModeSelector!.CheckedButtonId == Resource.Id.btnOnline))
        {
            this.ClickedStart = true;
            this.PermissionManager?.RequestAccess();
            return;
        }

        base.StartActivity(new Intent(this.Activity!, typeof(ChessActivity)));
    }
}
