using Android.Content;

namespace Chess.Util;

public static class SharedPrefExtensions
{
    public static ISharedPreferences? GetMaterialYouThemePreference(this Activity app, out bool MaterialYouThemePreference)
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

    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> to, params Dictionary<TKey, TValue>[] toBeMerged) where TKey : notnull
        where TValue : notnull
    {
        foreach (var dictionary in toBeMerged)
        {
            foreach (var kvp in dictionary)
            {
                if (!to.ContainsKey(kvp.Key))
                {
                    to[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}