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
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Chess.App.Common;

public static class Extensions
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

    public static NearbyConnections RegisterNearbyPermissionsManager(this ComponentActivity activity, Action<bool> OnRequestCallback)
    {
        var callback = new RequestPermissionsCallback(OnRequestCallback, activity.CheckSelfPermission);
        return new(activity.RegisterForActivityResult(new RequestMultiplePermissions(), callback), callback);
    }

    public static MediaAccess RegisterMediaPermissionsManager(this Fragment fragment, Action<bool> OnRequestCallback)
    {
        var callback = new RequestPermissionsCallback(OnRequestCallback, fragment.Activity!.CheckSelfPermission);
        return new(fragment.RegisterForActivityResult(new RequestMultiplePermissions(), callback), callback);
    }

    public static CameraAccess RegisterCameraPermissionManager(this Fragment fragment, Action<bool> OnRequestCallback)
    {
        var callback = new RequestPermissionCallback(OnRequestCallback, fragment.Activity!.CheckSelfPermission);
        return new(fragment.RegisterForActivityResult(new RequestPermission(), callback), callback);
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
