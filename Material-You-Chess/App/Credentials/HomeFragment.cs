using Android.Content;
using Android.Views;
using Java.Lang;

namespace Chess.App.Credentials;

internal class HomeFragment : AndroidX.Fragment.App.Fragment
{
    private IHomeFragmentCallback? listener;
    private Button? Logout;
    private TextView? SignedInText;


    private const string LOGGED_IN_THROUGH_PASSWORD = "Logged in successfully through password";
    private const string LOGGED_IN_THROUGH_PASSKEYS = "Logged in successfully through passkeys";

    public override void OnAttach(Context context)
    {
        base.OnAttach(context);
        try
        {
            listener = context as IHomeFragmentCallback;
        }
        // The activity does not implement the listener. 
        catch (ClassCastException) { }
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.fragment_home, container, false)!;

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        this.ConfigureSignedInText();
        this.Logout = view.FindViewById<Button>(Resource.Id.logout);
        this.Logout!.Click += (_, _) => listener?.Logout();
    }

    private void ConfigureSignedInText()
    {
        if (DataProvider.IsSignedInThroughPasskeys())
        {
            this.SignedInText!.Text = LOGGED_IN_THROUGH_PASSKEYS;
            return;
        }

        this.SignedInText!.Text = LOGGED_IN_THROUGH_PASSWORD;
    }

}

public interface IHomeFragmentCallback
{
    void Logout();
}
