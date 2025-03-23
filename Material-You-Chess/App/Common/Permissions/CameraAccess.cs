using System.Diagnostics.CodeAnalysis;
using Android;
using Android.Content.PM;
using AndroidX.Activity.Result;

namespace Chess.App.Common.Permissions;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class CameraAccess(ActivityResultLauncher requestLauncher, RequestPermissionCallback callback) : IPermissionManager
{
    public (Permission IsGranted, string Permission) Camera = (Permission.Denied, Manifest.Permission.Camera);

    public bool HasAccess()
    {
        Camera.IsGranted = callback.CheckSelfPermission(Camera.Permission);

        return Camera.IsGranted == Permission.Granted;
    }

    public void RequestAccess()
    {
        if (HasAccess())
        {
            callback.OnRequestCallback(true);
            return;
        }

        requestLauncher.Launch(Camera.Permission);
    }
}
