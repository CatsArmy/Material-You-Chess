using Android.Content.PM;
using Chess.App.Common.ActivityResult;
using Java.Util;
using JavaBool = Java.Lang.Boolean;

namespace Chess.App.Common;

/// <param name="callback">
/// <see langword="when" /> <paramref name="callback"/> <see langword="is" /> <see langword="false" />: <br />
/// Explain to the user that the feature is unavailable because the
/// feature requires a permission that the user has denied. At the
/// same time, respect the user's decision. Don't link to system
/// settings in an effort to convince the user to change their decision.
/// <br /> <see langword="when" /> <paramref name="callback"/> <see langword="is" /> <see langword="true" />: <br />
/// Permission is granted. Continue the action or workflow in your app.
/// </param>
public class RequestPermissionsCallback(Action<bool> callback, Func<string?, Permission> checkSelf) : ActivityResultCallback<IMap>(permissions
    => callback(!permissions!.Values().Cast<bool>().ToArray().Contains(false)))
{
    public void OnRequestCallback(bool isGranted) => callback(isGranted);
    public Permission CheckSelfPermission(string? permission) => checkSelf(permission);
}

/// <param name="callback">
/// <see langword="when" /> <paramref name="callback"/> <see langword="is" /> <see langword="false" />: <br />
/// Explain to the user that the feature is unavailable because the
/// feature requires a permission that the user has denied. At the
/// same time, respect the user's decision. Don't link to system
/// settings in an effort to convince the user to change their decision.
/// <br /> <see langword="when" /> <paramref name="callback"/> <see langword="is" /> <see langword="true" />: <br />
/// Permission is granted. Continue the action or workflow in your app.
/// </param>
public class RequestPermissionCallback(Action<bool> callback, Func<string?, Permission> checkSelf) : ActivityResultCallback<JavaBool>(isGranted
    => callback(isGranted!.BooleanValue()))
{
    public void OnRequestCallback(bool isGranted) => callback(isGranted);
    public Permission CheckSelfPermission(string? permission) => checkSelf(permission);
}
