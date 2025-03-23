using Android.Content.PM;

namespace Chess.App.Common;

public interface IPermissionManager
{
    public bool HasAccess();
    public void RequestAccess();
}

public interface IPermissionsManager : IPermissionManager
{
    public (Permission IsGranted, string Permission)[] Permissions { get; }
}
