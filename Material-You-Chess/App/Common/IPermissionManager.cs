using Android.Content.PM;

namespace Chess.App.Common;

public interface IPermissionManager
{
    public (Permission IsGranted, string Permission)[] Permissions { get; }
    public bool HasAccess();
    public void RequestAccess();
}
