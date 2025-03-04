//using Android.Content;
//using Android.Views;
//using Java.Lang;

//namespace Chess.App.Credentials;

//internal class HomeFragment
//{
//    private FragmentHomeBinding binding;
//    private IHomeFragmentCallback listener;


//    private const string LOGGED_IN_THROUGH_PASSWORD = "Logged in successfully through password";
//    private const string LOGGED_IN_THROUGH_PASSKEYS = "Logged in successfully through passkeys";



//    override void OnAttach(Context context)
//    {
//        base.OnAttach(context);
//        try
//        {
//            listener = context as IHomeFragmentCallback;
//        }
//        catch (ClassCastException castException)
//        {
//            /** The activity does not implement the listener.  */
//        }
//    }

//    override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
//    {
//        this.binding = FragmentHomeBinding.Inflate(inflater, container, false);
//        return binding.root;
//    }

//    override void OnViewCreated(View view, Bundle? savedInstanceState)
//    {
//        base.OnViewCreated(view, savedInstanceState);

//        ConfigureSignedInText();

//        binding.logout.setOnClickListener {
//            listener.Logout();
//        }
//    }

//    private void ConfigureSignedInText()
//    {
//        //if (DataProvider.IsSignedInThroughPasskeys())
//        //{
//        //    binding.signedInText.text = LOGGED_IN_THROUGH_PASSKEYS;
//        //    return;
//        //}

//        binding.signedInText.text = LOGGED_IN_THROUGH_PASSWORD;
//    }

//}
//public interface IHomeFragmentCallback
//{
//    void Logout();
//}
