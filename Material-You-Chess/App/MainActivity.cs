using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Chess.App.Common.Extensions;
using Firebase;
using Firebase.AppCheck;
using Firebase.AppCheck.PlayIntegrity;
using Firebase.Auth;
using Google.Android.Material.Navigation;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Chess.App;

[Activity(MainLauncher = true,
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = Android.Content.PM.ScreenOrientation.UserPortrait,
    EnableOnBackInvokedCallback = true
)]
public class MainActivity : AppCompatActivity
{
    /// <summary> The navigation bar used to let the user navigate between the profile fragment and main fragment </summary>
    public NavigationBarView? NavigationBar;

    /// <summary>the a reference to the ProfileFragment</summary>
    public MainFragment? Main;

    /// <summary>the a reference to the ProfileFragment</summary>
    public ProfileFragment? Profile;

    /// <summary> The container view used to display the MainFragment or ProfileFragment above</summary>
    public FragmentContainerView? FragmentContainer;

    /// <summary>A reference to the FirebaseAuth.Instance with the firebase AppCheck applied</summary>
    public FirebaseAuth? Auth;

    /// <summary> getter property used as a short-hand to get the my preconfigured fragment transaction </summary>
    private FragmentTransaction? FragmentTransaction => this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true);

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        this.SetContentView(Resource.Layout.main_activity);
        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.FragmentContainer = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);
        this.NavigationBar!.ItemSelected += this.NavigateToItemSelected;
        this.Main = new MainFragment();
        this.Profile = new ProfileFragment();
        this.FragmentTransaction?.Add(this.FragmentContainer!.Id, this.Main).Commit();

        var app = FirebaseApp.InitializeApp(this)!;
        var check = FirebaseAppCheck.GetInstance(app);
        check.InstallAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.Instance);
        this.Auth = FirebaseAuth.GetInstance(app);
        this.Auth!.AuthState += this.OnSignOut;
        this.RegisterComponents();
    }

    /// <summary> navigates you back to the main fragment as you are no longer logged in </summary>
    private void OnSignOut(object? sender, FirebaseAuth.AuthStateEventArgs e)
    {
        if (e.Auth.CurrentUser is not null) return;

        this.NavigationBar!.SelectedItemId = Resource.Id.play;
    }

    /// <summary>Navigates the user to the selected fragment</summary>
    private void NavigateToItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (this.NavigationBar?.SelectedItemId == e.Item.ItemId) return;

        switch (e.Item.ItemId)
        {
            case Resource.Id.play:
                this.FragmentTransaction?.Replace(this.FragmentContainer!.Id, this.Main!).Commit();
                break;
            case Resource.Id.profile:
                this.FragmentTransaction?.Replace(this.FragmentContainer!.Id, this.Profile!).Commit();
                break;
        }
    }
}
