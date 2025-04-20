using Android;
using Android.Content.PM;

namespace Material.You.Chess.App.Common.Permissions;

public interface INearbyPermissions
{
    public const string NearbyWifiDevices = Manifest.Permission.NearbyWifiDevices;
    public const string AccessWifiState = Manifest.Permission.AccessWifiState;
    public const string ChangeWifiState = Manifest.Permission.ChangeWifiState;
    public const string Bluetooth = Manifest.Permission.Bluetooth;
    public const string BluetoothAdmin = Manifest.Permission.BluetoothAdmin;
    public const string BluetoothAdvertise = Manifest.Permission.BluetoothAdvertise;
    public const string BluetoothConnect = Manifest.Permission.BluetoothConnect;
    public const string BluetoothScan = Manifest.Permission.BluetoothScan;
    public const string AccessCoarseLocation = Manifest.Permission.AccessCoarseLocation;
    public const string AccessFineLocation = Manifest.Permission.AccessFineLocation;

    public (Permission IsGranted, string Permission)[] Permissions { get; }

    public bool HasAccess();
    public void RequestAccess();
}
