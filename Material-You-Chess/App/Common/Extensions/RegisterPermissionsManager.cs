using AndroidX.Activity;
using Chess.App.Common.Permissions;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
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
}
