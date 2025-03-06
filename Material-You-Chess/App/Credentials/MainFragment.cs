using Android.Content;
using Android.Views;
using Java.Lang;

namespace Chess.App.Credentials;

class MainFragment : AndroidX.Fragment.App.Fragment
{
    private IMainFragmentCallback? listener;
    private Button? signUp;
    private Button? signIn;

    public override void OnAttach(Context context)
    {
        base.OnAttach(context);
        try
        {
            listener = context as IMainFragmentCallback;
        }
        //The activity does not implement the listener.  
        catch (ClassCastException) { }
    }

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.fragment_main, container, false);


    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.signUp!.Click += (_, _) => listener?.Signup();


        this.signIn!.Click += (_, _) => listener?.SignIn();
    }

    public override void OnDestroyView()
    {
        base.OnDestroyView();
        this.signUp = null;
        this.signIn = null;
    }
}

public interface IMainFragmentCallback
{
    public void Signup();
    public void SignIn();
}
