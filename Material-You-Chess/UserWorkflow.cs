using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Navigation;
using Microsoft.Maui.ApplicationModel;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class Main : AppCompatActivity
{
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }

    public FragmentContainerView? Fragment;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        this.SetContentView(Resource.Layout._main_activity_);
        this.NavigationBar = base.FindViewById<NavigationBarView>(Resource.Id.navigation_bar);
        this.NavigationBar!.ItemSelected += this.NavigationBar_ItemSelected;
        this.PlayItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_1);
        this.ProfileItem = this.NavigationBar.Menu.FindItem(Resource.Id.item_2);
        this.Fragment = base.FindViewById<FragmentContainerView>(Resource.Id.fragment_container_view);
        this.SupportFragmentManager?.BeginTransaction()
            ?.Add(this.Fragment!.Id, new MainFragment())?.Commit();
        //profileItem.SetIcon();
        FirebaseAuth.Instance.SignOut();
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

public interface IMainNavigationBar
{
    public NavigationBarView? NavigationBar { get; set; }
    public IMenuItem? PlayItem { get; set; }
    public IMenuItem? ProfileItem { get; set; }
}

public class MainFragment : AndroidX.Fragment.App.Fragment
{
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.main_fragment, container, false);
}

public class ProfileFragment() : AndroidX.Fragment.App.Fragment()
{
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.__profile_fragment__, container, false);
}

public class SelectAccountMethodFragment() : AndroidX.Fragment.App.Fragment()
{
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.account_select_method, container, false);
}

public class SignInFragment() : AndroidX.Fragment.App.Fragment()
{
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.sign_in, container, false);
}

public class SignUpFragment() : AndroidX.Fragment.App.Fragment()
{
    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.sign_up, container, false);
}

public class AuthUIActivity : AppCompatActivity
{
    public ActivityResultCallback<FirebaseAuthUIAuthenticationResult> Callback;

}