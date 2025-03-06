using Android.Content;

namespace Chess.App.Credentials;

public static class DataProvider
{

    private static ISharedPreferences? sharedPreference;
    private static ISharedPreferencesEditor? editor;

    private const string IS_SIGNED_IN = "isSignedIn";
    private const string IS_SIGNED_IN_THROUGH_PASSKEYS = "isSignedInThroughPasskeys";
    private const string PREF_NAME = "CREDMAN_PREF";

    public static void InitSharedPref(Context context)
    {
        sharedPreference = context.ApplicationContext?.GetSharedPreferences(PREF_NAME, FileCreationMode.Private);
        editor = sharedPreference?.Edit();
    }

    //Set if the user is signed in or not
    public static void ConfigureSignedInPref(bool flag)
    {
        editor?.PutBoolean(IS_SIGNED_IN, flag);
        editor?.Commit();
    }

    //Set if signed in through passkeys or not
    public static void SetSignedInThroughPasskeys(bool flag)
    {
        editor?.PutBoolean(IS_SIGNED_IN_THROUGH_PASSKEYS, flag);
        editor?.Commit();
    }

    public static bool IsSignedIn()
    {
        return sharedPreference!.GetBoolean(IS_SIGNED_IN, false);
    }

    public static bool IsSignedInThroughPasskeys()
    {
        return sharedPreference!.GetBoolean(IS_SIGNED_IN_THROUGH_PASSKEYS, false);
    }
}

