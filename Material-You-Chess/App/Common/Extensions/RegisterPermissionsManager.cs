using AndroidX.Activity;
using Chess.App.Common.Permissions;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    public static NearbyConnections RegisterNearbyPermissionsManager(this ComponentActivity activity, Action<bool> OnRequestCallback)
    {
        var callback = new RequestPermissionsCallback(OnRequestCallback, activity.CheckCallingOrSelfPermission);
        return new(activity.RegisterForActivityResult(new RequestMultiplePermissions(), callback), callback);
    }
}
