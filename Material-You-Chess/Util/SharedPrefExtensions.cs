using Android.Content;
using AndroidX.AppCompat.App;

namespace Chess.Util;

public static class SharedPrefExtensions
{
    public static ISharedPreferences? GetMaterialYouThemePreference(this AppCompatActivity app, out bool MaterialYouThemePreference)
    {
        ISharedPreferences? sharedPref = app.GetPreferences(FileCreationMode.Private);
        MaterialYouThemePreference = true;
        if (sharedPref!.Contains(nameof(MaterialYouThemePreference)))
        {
            MaterialYouThemePreference = sharedPref.GetBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference);
            return sharedPref;
        }
        var editor = sharedPref.Edit();
        editor?.PutBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference)?.Commit();
        editor?.Apply();
        return sharedPref;
    }
}