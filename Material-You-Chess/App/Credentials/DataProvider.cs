//namespace Chess.App.Credentials;

//internal class DataProvider
//{

//    private ISharedPreferences? sharedPreference;
//    private ISharedPreferencesEditor? editor;

//    private const string IS_SIGNED_IN = "isSignedIn";
//    private const string IS_SIGNED_IN_THROUGH_PASSKEYS = "isSignedInThroughPasskeys";
//    private const string PREF_NAME = "CREDMAN_PREF";

//    public void InitSharedPref(Context context)
//    {
//        sharedPreference = context.ApplicationContext?.GetSharedPreferences(PREF_NAME, FileCreationMode.Private);
//        editor = sharedPreference?.Edit();
//    }

//    //Set if the user is signed in or not
//    public void ConfigureSignedInPref(bool flag)
//    {
//        editor?.PutBoolean(IS_SIGNED_IN, flag);
//        editor?.Commit();
//    }

//    //Set if signed in through passkeys or not
//    public void SetSignedInThroughPasskeys(bool flag)
//    {
//        editor?.PutBoolean(IS_SIGNED_IN_THROUGH_PASSKEYS, flag);
//        editor?.Commit();
//    }

//    public bool IsSignedIn()
//    {
//        return sharedPreference!.GetBoolean(IS_SIGNED_IN, false);
//    }

//    public bool IsSignedInThroughPasskeys()
//    {
//        return sharedPreference!.GetBoolean(IS_SIGNED_IN_THROUGH_PASSKEYS, false);
//    }
//}

