//using AndroidX.AppCompat.App;

//namespace Chess.App.Credentials;

//public class MainActivity : AppCompatActivity, MainFragmentCallback, HomeFragmentCallback, SignInFragmentCallback, SignUpFragmentCallback
//{

//    protected override void OnCreate(Bundle? savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);

//        binding = ActivityMainBinding.Inflate(layoutInflater);
//        SetContentView(binding.root);

//        DataProvider.InitSharedPref(applicationContext)

//        if (DataProvider.isSignedIn())
//        {
//            showHome()
//        }
//        else
//        {
//            loadMainFragment()
//        }
//    }

//    override fun signup()
//    {
//        loadFragment(SignUpFragment(), false)
//    }

//    override fun signIn()
//    {
//        loadFragment(SignInFragment(), false)
//    }

//    override fun logout()
//    {
//        supportFragmentManager.popBackStack("home", FragmentManager.POP_BACK_STACK_INCLUSIVE)
//        loadMainFragment()
//    }

//    private fun loadMainFragment()
//    {
//        supportFragmentManager.popBackStack()
//        loadFragment(MainFragment(), false)
//    }

//    override fun showHome()
//    {
//        supportFragmentManager.popBackStack()
//        loadFragment(HomeFragment(), true, "home")
//    }

//    private fun loadFragment(fragment: Fragment, flag: Boolean, backstackString: String? = null)
//    {
//        DataProvider.configureSignedInPref(flag)
//        supportFragmentManager.beginTransaction().replace(id.fragment_container, fragment)
//            .addToBackStack(backstackString).commit()
//    }

//    override fun onBackPressed()
//    {
//        if (DataProvider.isSignedIn() || supportFragmentManager.backStackEntryCount == 1)
//        {
//            finish()
//        }
//        else
//        {
//            super.onBackPressed()
//        }
//    }
//}