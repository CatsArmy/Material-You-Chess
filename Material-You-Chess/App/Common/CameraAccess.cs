using System.Diagnostics.CodeAnalysis;
using Android;
using Android.Content.PM;
using AndroidX.Activity.Result;

namespace Chess.App.Common;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class CameraAccess(ActivityResultLauncher requestLauncher, RequestPermissionCallback callback) : IPermissionManager
{
    public (Permission IsGranted, string Permission) Camera = (Permission.Denied, Manifest.Permission.Camera);

    public bool HasAccess()
    {
        this.Camera.IsGranted = callback.CheckSelfPermission(this.Camera.Permission);

        return this.Camera.IsGranted == Permission.Granted;
    }

    public void RequestAccess()
    {
        if (this.HasAccess())
        {
            callback.OnRequestCallback(true);
            return;
        }

        requestLauncher.Launch(this.Camera.Permission);
    }
}
