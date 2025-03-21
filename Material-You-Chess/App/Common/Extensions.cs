using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby.Connection;
using Android.Views.Animations;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using Chess.App.Common.ActivityResult;
using Google.Android.Material.FloatingActionButton;
using Java.Util;

namespace Chess.App.Common;

public static class Extensions
{
    public static ISharedPreferences? GetMaterialYouThemePreference(this Android.App.Activity app, out bool MaterialYouThemePreference)
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

    public static void Spin(this ExtendedFloatingActionButton fab)
    {
        if (fab.Extended)
            return;

        fab.Rotation = 0;

        fab.Animate()?.Rotation(360).WithLayer().SetDuration(1000).SetInterpolator(new AccelerateDecelerateInterpolator()).Start();
    }

    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> to, params Dictionary<TKey, TValue>[] merge) where TKey : notnull where TValue : notnull
    {
        foreach (var dictionary in merge)
            foreach (var kvp in dictionary)
            {
                if (to.ContainsKey(kvp.Key))
                    continue;

                to[kvp.Key] = kvp.Value;
            }
    }

    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where I : Java.Lang.Object
        where O : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where I : Java.Lang.Object
        where O : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback @callback)
        where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback @callback)
        where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where O : Java.Lang.Object
        => @base.RegisterForActivityResult(contract, callback)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where O : Java.Lang.Object
        => @base.RegisterForActivityResult(contract, callback)!;

    /// <summary> Transforms a <see cref="Statuses"/> into a English-readable message for logging. </summary>
    /// <param name="status">The current status. </param>
    /// <returns> A readable String.eg. [404] File not found. </returns>
    public static string Status(this Statuses status)
    {
        string msg = (status.StatusMessage == null) switch
        {
            true => ConnectionsStatusCodes.GetStatusCodeString(status.StatusCode),
            false => status.StatusMessage
        };
        return Java.Lang.String.Format(Locale.Us!, "[%d]%s", status.StatusCode, msg!).ToString();
    }
}
