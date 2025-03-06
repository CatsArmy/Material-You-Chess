using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Google.Android.Material.Navigation;
using Microsoft.Maui.ApplicationModel;

namespace Chess;

//[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class Main() : AppCompatActivity(Resource.Layout._main_activity_), IMainNavigationBar
{
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }

    public FragmentContainerView? Fragment;

    public override void OnCreate(Bundle? savedInstanceState, PersistableBundle? persistentState)
    {
        base.OnCreate(savedInstanceState, persistentState);
        Platform.Init(this, savedInstanceState);

        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.NavigationBar!.ItemSelected += this.NavigationBar_ItemSelected;
        this.PlayItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_1);
        this.ProfileItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_2);
        this.Fragment = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);
        //profileItem.SetIcon();
    }

    private void NavigationBar_ItemSelected(object? sender, NavigationBarView.ItemSelectedEventArgs e)
    {
        if (e.Item.ItemId == this.PlayItem?.ItemId)
        {
            this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Add(this.Fragment!.Id, new MainFragment(this))?.Commit();
        }
        else if (e.Item.ItemId == this.ProfileItem?.ItemId)
        {
            _ = (Plugin.Firebase.Auth.CrossFirebaseAuth.Current.CurrentUser is null) switch
            {
                true => this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Add(this.Fragment!.Id, new SelectAccountMethodFragment(this))?.Commit(),

                false => this.SupportFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Add(this.Fragment!.Id, new ProfileFragment(this))?.Commit()
            };
        }
    }
}

public interface IMainNavigationBar
{
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }
}

public class MainFragment(IMainNavigationBar main) : AndroidX.Fragment.App.Fragment(Resource.Layout.main_fragment)
{
    public IMainNavigationBar Main = main;

}

public class ProfileFragment(IMainNavigationBar main) : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    public IMainNavigationBar Main = main;

}

public class SelectAccountMethodFragment(IMainNavigationBar main) : AndroidX.Fragment.App.Fragment(Resource.Layout.account_select_method)
{
    public IMainNavigationBar Main = main;
}

public class SignInFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.sign_in)
{

}

public class SignUpFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.sign_up)
{

}