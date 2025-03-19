using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Views;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase;
using Firebase.AppCheck;
using Firebase.AppCheck.PlayIntegrity;
using Firebase.Auth;
using FirebaseUI.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class Main_Activity : AppCompatActivity
{
    public static Main_Activity? Instance { get; set; }
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }
    public FragmentContainerView? FragmentContainer { get; set; }
    public ActivityResultLauncher? SignInLauncher;
    public ProfileFragment? Profile;
    public MainFragment? Main;
    private FirebaseAuth? Auth;
    public bool MaterialYouThemePreference
    {
        get; set
        {
            field = value;

            base.SetTheme(this.ThemeId);
        }
    } = true;

    public int ThemeId => this.MaterialYouThemePreference ? Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar
            : Resource.Style.AppTheme_Material3_DayNight_NoActionBar;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Main_Activity.Instance = this;
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        this.SetContentView(Resource.Layout._main_activity_);
        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.FragmentContainer = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);
        this.PlayItem = this.NavigationBar!.Menu.FindItem(Resource.Id.item_1);
        this.ProfileItem = this.NavigationBar!.Menu.FindItem(Resource.Id.item_2);
        this.NavigationBar!.ItemSelected += this.NavigationBar_ItemSelected;
        this.Main = new();
        this.Profile = new();
        this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
            .Add(this.FragmentContainer!.Id, this.Main).Commit();

        this.SignInLauncher = base.RegisterForActivityResult(contract: new FirebaseAuthUIActivityResultContract(),
            callback: new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));

        //var app = FirebaseApp.InitializeApp(this)!;
        //FirebaseAppCheck.GetInstance(app).InstallAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.Instance);

        //this.Auth = FirebaseAuth.GetInstance(app);
        this.Auth = FirebaseAuth.Instance;
        this.Auth.AuthState += (s, e) =>
        {
            if (this.Main?.Online is null)
                return;

            this.Main.Online.Enabled = e.Auth.CurrentUser is not null;
        };

        this.Main.IsLoggedIn = this.Auth.CurrentUser is not null;
    }

    private void OpenSignInIntentActivity()
    {
        List<AuthUI.IdpConfig> providers = [
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
            new AuthUI.IdpConfig.GoogleBuilder().Build()
        ];

        // Create and launch sign-in intent
        var signInIntent = AuthUI.Instance.CreateSignInIntentBuilder()
            //.EnableAnonymousUsersAutoUpgrade()
            //.SetAvailableProviders(providers)
            //.SetAlwaysShowSignInMethodScreen(true)
            .SetTheme(this.ThemeId)
            .SetLogo(Resource.Drawable.ic_launcher_foreground)
            .SetCredentialManagerEnabled(true)
            .SetLockOrientation(true)
            .Build();

        // Attempt to Sign In/Up the device
        this.SignInLauncher?.Launch(signInIntent);
    }

    private void OnSignInResult(FirebaseAuthUIAuthenticationResult? result)
    {
        var resultCode = result?.ResultCode.IntValue();
        var response = result?.IdpResponse;

        // Successfully signed in
        if (resultCode == ((int)Result.Ok))
        {
            this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
                .Replace(this.FragmentContainer!.Id, this.Profile!).Commit();
        }
        else
        {
            //Sign in failed
            if (response == null)
            {
                // User pressed back button
                Snackbar.Make(this.FragmentContainer!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
                return;
            }

            if (response?.Error?.ErrorCode == ErrorCodes.NoNetwork)
            {
                Snackbar.Make(this.FragmentContainer!, Resource.String.no_internet_connection, Snackbar.LengthLong);
                return;
            }

            Snackbar.Make(this.FragmentContainer!, Resource.String.unknown_error, Snackbar.LengthLong).Show();
            this.OpenSignInIntentActivity();
        }
    }

    private void NavigationBar_ItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (e.Item.ItemId == this.PlayItem?.ItemId)
        {
            this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
                .Replace(this.FragmentContainer!.Id, this.Main!).Commit();
            return;
        }

        if (e.Item.ItemId == this.ProfileItem?.ItemId)
        {
            if (this.Auth?.CurrentUser is not null)
            {
                this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
                    .Replace(this.FragmentContainer!.Id, this.Profile!).Commit();
                return;
            }

            this.OpenSignInIntentActivity();
        }
    }
}
