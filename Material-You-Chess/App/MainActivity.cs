using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Chess.App.Common.Extensions;
using Firebase;
using Firebase.AppCheck;
using Firebase.AppCheck.PlayIntegrity;
using Firebase.Auth;
using Google.Android.Material.Navigation;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Chess.App;

[Activity(MainLauncher = true,
    Label = "@string/app_name",
    Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar",
    ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
    EnableOnBackInvokedCallback = true
)]
public class MainActivity : AppCompatActivity
{
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? MainItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }

    public MainFragment? Main { get; set; }
    public ProfileFragment? Profile { get; set; }

    /// <summary> The container for the MainFragment or ProfileFragment above</summary>
    public FragmentContainerView? FragmentContainer { get; set; }
    /// <summary>A reference to the FirebaseAuth.Instance with the firebase AppCheck applied</summary>
    public FirebaseAuth? Auth;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        this.SetContentView(Resource.Layout.main_activity);
        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.FragmentContainer = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);
        this.MainItem = this.NavigationBar!.Menu.FindItem(Resource.Id.play);
        this.ProfileItem = this.NavigationBar!.Menu.FindItem(Resource.Id.profile);
        this.NavigationBar!.ItemSelected += this.NavigationBar_ItemSelected;
        this.Main = new MainFragment();
        this.Profile = new ProfileFragment();
        this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
            .Add(this.FragmentContainer!.Id, this.Main, nameof(this.Main)).Commit();

        var app = FirebaseApp.InitializeApp(this)!;
        var check = FirebaseAppCheck.GetInstance(app);
        check.InstallAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.Instance);
        this.Auth = FirebaseAuth.GetInstance(app);
        this.Auth!.AuthState += this.OnSignOut;
        this.RegisterComponents();
    }

    private void OnSignOut(object? sender, FirebaseAuth.AuthStateEventArgs e)
    {
        if (e.Auth.CurrentUser is null)
        {
            this.NavigationBar!.SelectedItemId = this.MainItem!.ItemId;
        }
    }

    /// <summary>
    /// Opens the page based on the bottom nav bar item user selected
    /// if (e.Item.ItemId) == MainItem.ItemId it will open the MainFragment
    /// else if (e.Item.ItemId) == ProfileItem.ItemId it will open the ProfileFragment if the user is logged in 
    /// else it will open the sign in/up page
    /// </summary>
    private void NavigationBar_ItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (this.NavigationBar?.SelectedItemId == e.Item.ItemId)
            return;

        if (e.Item.ItemId == this.MainItem?.ItemId)
        {
            this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
                .Replace(this.FragmentContainer!.Id, this.Main!, nameof(this.Main)).Commit();
            return;
        }

        if (e.Item.ItemId == this.ProfileItem?.ItemId)
        {
            this.SupportFragmentManager?.BeginTransaction().SetReorderingAllowed(true)
                .Replace(this.FragmentContainer!.Id, this.Profile!, nameof(this.Profile)).Commit();
            return;
        }
    }

}
