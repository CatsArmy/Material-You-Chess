using Android.Content;
using Android.Views;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Java.Util;
using Material.You.Chess.App.Common.ActivityResult;
using Material.You.Chess.App.Common.Permissions;
using Material.You.Chess.Game;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Material.You.Chess.App;

public class MainFragment() : Fragment(Resource.Layout.main_fragment)
{
    /// <summary> Manages the online and local buttons to ensure only one is checked at a time </summary>
    public MaterialButtonToggleGroup? GameModeSelector { get; private set; }
    public MaterialButton? Online { get; private set; }
    public MaterialButton? Local { get; private set; }
    public ExtendedFloatingActionButton? Start { get; private set; }
    public const string IntentArgs = $"{nameof(ChessActivity)}{nameof(IntentArgs)}";
    public const string IsNetworked = nameof(IsNetworked);
    public const string Uid = $"{UserClient}:{nameof(Uid)}";
    public const string Username = $"{UserClient}:{nameof(Username)}";
    private const string UserClient = $"Firebase{nameof(UserClient)}";

    /// <summary> when settings the logged in value enables/disables the online mode button </summary>
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
    private FirebaseUser? User;
    private NearbyConnections? PermissionManager;

    private static bool GrantedAll(IMap permissions) => !permissions.Values().Cast<bool>().ToArray().Contains(false);
    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var onResult = new ActivityResultCallback<IMap>(this.OnPermissionsResult);
        var resultLauncher = this.RegisterForActivityResult(new RequestMultiplePermissions(), onResult);

        this.PermissionManager = new(resultLauncher, this.Activity);
    }

    /// <summary> cleanup auth state listeners to prevent duplicate calls to the on auth state </summary>
    public override void OnDestroy()
    {
        this.Auth!.AuthState -= this.OnAuthState;
        base.OnDestroy();
    }

    /// <summary> binds the views and inits the ui loop </summary>
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
        this.User = this.Auth!.CurrentUser;
        this.IsLoggedIn = this.User is not null;
        this.Auth!.AuthState += this.OnAuthState;
    }

    /// <summary> updates the ui to indicate to the user that starting a online game is available </summary>
    private void OnCheckOnline(object? sender, EventArgs e) => this.PermissionManager?.RequestAccess();

    /// <summary> updates the ui to indicate to the user that starting a local game is available </summary>
    private void OnCheckLocal(object? sender, EventArgs e)
    {
        this.Start!.SetIconResource(Resource.Drawable.group);
        this.Start!.Text = "Play";
    }


    /// <summary> starts the game activity with the selected mode </summary>
    private void StartGame(object? sender, EventArgs e)
    {
        if (this.GameModeSelector!.CheckedButtonId == Resource.Id.btnOnline)
        {
            // On request granted opens the game activity
            this.ClickedStart = true;
            this.PermissionManager?.RequestAccess();
            return;
        }
        // open a local game activity
        base.StartActivity(new Intent(this.Activity!,
            typeof(ChessActivity)).PutExtra(IsNetworked, value: "false"));
    }

    /// <summary> wrapper for the HandlePermissionResult function </summary>
    private void OnPermissionsResult(IMap? permissions) => this.HandlePermissionResult(GrantedAll(permissions!));

    /// <summary>
    /// Updates the ui based on the permission that was might have been granted
    /// also starts the networked chess activity if the permission requester was the onclick start</summary>
    /// <param name="isGranted"> is true when all of the requested permissions granted </param>
    private void HandlePermissionResult(bool isGranted)
    {
        if (!isGranted)
        {
            // informs the user that to start the online mode it requires the permission to be granted
            this.Online!.SetIconResource(Resource.Drawable.nearby_off);
            this.Start!.SetIconResource(Resource.Drawable.nearby_error);
            this.Start!.Text = "Missing Permissions";
            return;
        }
        // revert the ui changes from above when the permission is granted
        this.Online!.SetIconResource(Resource.Drawable.nearby);
        this.Start!.SetIconResource(Resource.Drawable.nearby);
        this.Start!.Text = "Play";
        if (this.ClickedStart)
        {
            this.ClickedStart = false;
            base.StartActivity(new Intent(this.Activity!,
                typeof(ChessActivity)).PutExtra(IsNetworked, value: "true"));
        }
    }

    /// <summary> update ui to enable/disable the online mode if the user is logged in </summary>
    private void OnAuthState(object? sender, FirebaseAuth.AuthStateEventArgs args)
    {
        if (this.Online is null)
            return;

        this.Online.Enabled = args.Auth.CurrentUser is not null;
    }
}
