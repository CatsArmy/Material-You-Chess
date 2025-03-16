using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Chess.App.Common;
using Firebase;
using Firebase.AppCheck;
using Firebase.AppCheck.PlayIntegrity;
using Firebase.Auth;
using Google.Android.Material.Navigation;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class Main : AppCompatActivity
{
    public static Main? Instance { get; set; }
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }
    public FragmentContainerView? Fragment { get; set; }

    public bool MaterialYouThemePreference
    {
        get; set
        {
            field = value;
            if (value == true)
            {
                base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);
            }
            if (value == false)
            {
                base.SetTheme(Resource.Style.AppTheme_Material3_DayNight_NoActionBar);
            }
        }
    } = true;

    public int ThemeId => this.MaterialYouThemePreference
            ? Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar
            : Resource.Style.AppTheme_Material3_DayNight_NoActionBar;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        Main.Instance = this;
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        this.SetContentView(Resource.Layout._main_activity_);
        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.NavigationBar!.ItemSelected += this.NavigationBar_ItemSelected;
        this.PlayItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_1);
        this.ProfileItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_2);
        this.Fragment = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);

        FirebaseApp.InitializeApp(this);
        FirebaseAppCheck firebaseAppCheck = FirebaseAppCheck.Instance;
        firebaseAppCheck.InstallAppCheckProviderFactory(PlayIntegrityAppCheckProviderFactory.Instance);


        this.SupportFragmentManager?.BeginTransaction()?.Add(this.Fragment!.Id, new MainFragment())?.Commit();

        //profileItem.SetIcon();
        //FirebaseAuth.Instance.SignOut();
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (FirebaseAuth.Instance == null)
        {
            Logger.Warn("FirebaseAuth.Instance is null");
            return;
        }

        var user = FirebaseAuth.Instance.CurrentUser;
        if (user is not null)
        {
            Logger.Warn(user.DisplayName ?? "No display name found");
            Logger.Warn(user.Email ?? "No email found");
            Logger.Warn(user.Uid ?? "No Uid found");
        }
        else
        {
            Logger.Warn("user is null");
        }
    }

    private void NavigationBar_ItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (e.Item.ItemId == this.PlayItem?.ItemId)
        {
            this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Replace(this.Fragment!.Id, new MainFragment())?.Commit();
        }

        else if (e.Item.ItemId == this.ProfileItem?.ItemId)
        {
            _ = (FirebaseAuth.Instance.CurrentUser is null) switch
            {
                true => this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Replace(this.Fragment!.Id, new SelectAccountMethodFragment())?.Commit(),

                false => this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Replace(this.Fragment!.Id, new ProfileFragment())?.Commit()
            };
        }
    }
}
