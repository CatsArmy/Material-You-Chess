using Android.Content;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    public static bool MaterialYouThemePreference(this ContextWrapper contextWrapper)
    {
        var themePref =
            contextWrapper.ApplicationContext!.GetSharedPreferences("Theme", FileCreationMode.Private)!;

        return themePref.GetBoolean(nameof(MaterialYouThemePreference), true);
    }

    public static void MaterialYouThemePreference(this ContextWrapper contextWrapper, bool value)
    {
        var themePref = contextWrapper.ApplicationContext!.GetSharedPreferences("Theme", FileCreationMode.Private)!.Edit();
        if (!themePref!.PutBoolean(nameof(MaterialYouThemePreference), value)!.Commit())
        {
            Logger.Debug("Failed to commit material you theme preference?");
        }
    }
}
