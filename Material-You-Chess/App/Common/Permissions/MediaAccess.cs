using System.Diagnostics.CodeAnalysis;
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity.Result;

namespace Chess.App.Common.Permissions;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class MediaAccess(ActivityResultLauncher requestLauncher, RequestPermissionsCallback callback) : IPermissionsManager
{
    public (Permission IsGranted, string Permission) Camera = (Permission.Denied, Manifest.Permission.Camera);
    public (Permission IsGranted, string Permission) ReadMediaVisualUserSelected = (Permission.Denied, Manifest.Permission.ReadMediaVisualUserSelected);
    public (Permission IsGranted, string Permission) ReadMediaImages = (Permission.Denied, Manifest.Permission.ReadMediaImages);
    public (Permission IsGranted, string Permission) ReadMediaVideo = (Permission.Denied, Manifest.Permission.ReadMediaVideo);
    public (Permission IsGranted, string Permission) ReadExternalStorage = (Permission.Denied, Manifest.Permission.ReadExternalStorage);

    public (Permission IsGranted, string Permission)[] Permissions => Build.VERSION.SdkInt switch
    {
        // Partial access on Android 14 (API level 34) or higher
        >= BuildVersionCodes.UpsideDownCake =>
                [this.ReadMediaVisualUserSelected, this.ReadMediaImages, this.ReadMediaVideo],

        // Full access on Android 13 (API level 33) or higher
        >= BuildVersionCodes.Tiramisu => [this.ReadMediaImages, this.ReadMediaVideo,],

        // Full access up to Android 12 (API level 32)
        _ => [this.ReadExternalStorage]
    };

    public bool HasAccess()
    {
        for (int i = 0; i < this.Permissions.Length; i++)
            this.Permissions[i].IsGranted = callback.CheckSelfPermission(this.Permissions[i].Permission);

        bool hasAccess = true;
        foreach (var permission in this.Permissions) if (permission.IsGranted == Permission.Denied)
                hasAccess = false;

        return hasAccess;
    }

    public void RequestAccess()
    {
        if (this.HasAccess())
        {
            callback.OnRequestCallback(true);
            return;
        }

        requestLauncher.Launch((string[])[.. from permission in this.Permissions select permission.Permission]);
    }
}
