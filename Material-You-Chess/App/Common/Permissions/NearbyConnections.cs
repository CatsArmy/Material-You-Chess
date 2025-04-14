using System.Diagnostics.CodeAnalysis;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity.Result;

namespace Chess.App.Common.Permissions;

public interface IPermissionManager
{
    public bool HasAccess();
    public void RequestAccess();
}

public interface IPermissionsManager : IPermissionManager
{
    public (Permission IsGranted, string Permission)[] Permissions { get; }
}


[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "False positive")]
public class NearbyConnections(ActivityResultLauncher requestLauncher, ContextWrapper? context) : IPermissionsManager
{
    public (Permission IsGranted, string Permission) NearbyWifiDevices = (Permission.Denied, Manifest.Permission.NearbyWifiDevices);
    public (Permission IsGranted, string Permission) AccessWifiState = (Permission.Denied, Manifest.Permission.AccessWifiState);
    public (Permission IsGranted, string Permission) ChangeWifiState = (Permission.Denied, Manifest.Permission.ChangeWifiState);
    public (Permission IsGranted, string Permission) Bluetooth = (Permission.Denied, Manifest.Permission.Bluetooth);
    public (Permission IsGranted, string Permission) BluetoothAdmin = (Permission.Denied, Manifest.Permission.BluetoothAdmin);
    public (Permission IsGranted, string Permission) BluetoothAdvertise = (Permission.Denied, Manifest.Permission.BluetoothAdvertise);
    public (Permission IsGranted, string Permission) BluetoothConnect = (Permission.Denied, Manifest.Permission.BluetoothConnect);
    public (Permission IsGranted, string Permission) BluetoothScan = (Permission.Denied, Manifest.Permission.BluetoothScan);
    public (Permission IsGranted, string Permission) AccessCoarseLocation = (Permission.Denied, Manifest.Permission.AccessCoarseLocation);
    public (Permission IsGranted, string Permission) AccessFineLocation = (Permission.Denied, Manifest.Permission.AccessFineLocation);

    public (Permission IsGranted, string Permission)[] Permissions => Build.VERSION.SdkInt switch
    {
        >= BuildVersionCodes.Tiramisu => [this.BluetoothScan,
            this.BluetoothAdvertise,
            this.BluetoothConnect,
            this.AccessWifiState,
            this.ChangeWifiState,
            this.NearbyWifiDevices,
        ],

        >= BuildVersionCodes.S => [this.BluetoothScan,
            this.BluetoothAdvertise,
            this.BluetoothConnect,
            this.AccessWifiState,
            this.ChangeWifiState,
            this.NearbyWifiDevices,
            this.AccessCoarseLocation,
            this.AccessFineLocation,
        ],

        >= BuildVersionCodes.Q => [this.Bluetooth,
            this.BluetoothAdmin,
            this.BluetoothConnect,
            this.AccessWifiState,
            this.ChangeWifiState,
            this.NearbyWifiDevices,
            this.AccessCoarseLocation,
            this.AccessFineLocation,
        ],

        _ => [this.Bluetooth,
            this.BluetoothAdmin,
            this.BluetoothConnect,
            this.AccessWifiState,
            this.ChangeWifiState,
            this.AccessCoarseLocation,
            this.AccessFineLocation,
        ]
    };

    public bool HasAccess()
    {
        if (context is null) return false;

        for (int i = 0; i < this.Permissions.Length; i++)
            this.Permissions[i].IsGranted = context.CheckCallingOrSelfPermission(this.Permissions[i].Permission);

        bool hasAccess = true;
        foreach (var permission in this.Permissions) if (permission.IsGranted == Permission.Denied)
                hasAccess = false;
        return this.NearbyWifiDevices.IsGranted == Permission.Granted || hasAccess;
    }

    public void RequestAccess()
    {
        if (this.HasAccess()) return;

        requestLauncher.Launch((string[])[.. from permission in this.Permissions select permission.Permission]);
    }
}
