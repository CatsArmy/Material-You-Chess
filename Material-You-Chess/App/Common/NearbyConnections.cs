using System.Diagnostics.CodeAnalysis;
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using Request = Android.Manifest.Permission;

namespace Chess.App.Common;

public interface IPermissionManager
{
    public bool HasAccess();
    public void RequestAccess();
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class NearbyConnections(ActivityResultLauncher requestLauncher,
    Action<bool> OnRequestCallback, Func<string?, Permission>? CheckSelfPermission)
    : IPermissionManager
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

    public bool HasAccess()
    {
        Build.VERSION.SdkInt switch
        {
            >= BuildVersionCodes.Tiramisu =>
            this.BluetoothScan.IsGranted,
            this.BluetoothAdvertise.IsGranted,
            this.BluetoothConnect.IsGranted,
            this.AccessWifiState.IsGranted,
            this.ChangeWifiState.IsGranted,
            this.NearbyWifiDevices.IsGranted,

            >= BuildVersionCodes.S =>
            this.BluetoothScan.IsGranted == Permission.Granted &&
            this.BluetoothAdvertise.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.NearbyWifiDevices.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted,

            >= BuildVersionCodes.Q =>
            this.Bluetooth.IsGranted == Permission.Granted &&
            this.BluetoothAdmin.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.NearbyWifiDevices.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted,

            _ => this.Bluetooth.IsGranted == Permission.Granted &&
            this.BluetoothAdmin.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted
        };

        return Build.VERSION.SdkInt switch
        {
            >= BuildVersionCodes.Tiramisu =>
            this.BluetoothScan.IsGranted == Permission.Granted &&
            this.BluetoothAdvertise.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.NearbyWifiDevices.IsGranted == Permission.Granted,

            >= BuildVersionCodes.S =>
            this.BluetoothScan.IsGranted == Permission.Granted &&
            this.BluetoothAdvertise.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.NearbyWifiDevices.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted,

            >= BuildVersionCodes.Q =>
            this.Bluetooth.IsGranted == Permission.Granted &&
            this.BluetoothAdmin.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.NearbyWifiDevices.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted,

            _ => this.Bluetooth.IsGranted == Permission.Granted &&
            this.BluetoothAdmin.IsGranted == Permission.Granted &&
            this.BluetoothConnect.IsGranted == Permission.Granted &&
            this.AccessWifiState.IsGranted == Permission.Granted &&
            this.ChangeWifiState.IsGranted == Permission.Granted &&
            this.AccessCoarseLocation.IsGranted == Permission.Granted &&
            this.AccessFineLocation.IsGranted == Permission.Granted
        };
    }

    public void RequestAccess()
    {
        if (this.HasAccess())
        {
            OnRequestCallback(true);
            return;
        }

        requestLauncher.Launch((string[])(Build.VERSION.SdkInt switch
        {
            >= BuildVersionCodes.Tiramisu => [this.BluetoothScan.Permission,
                this.BluetoothAdvertise.Permission,
                this.BluetoothConnect.Permission,
                this.AccessWifiState.Permission,
                this.ChangeWifiState.Permission,
                this.NearbyWifiDevices.Permission,],

            >= BuildVersionCodes.S => [this.BluetoothScan.Permission,
                this.BluetoothAdvertise.Permission,
                this.BluetoothConnect.Permission,
                this.AccessWifiState.Permission,
                this.ChangeWifiState.Permission,
                this.NearbyWifiDevices.Permission,
                this.AccessCoarseLocation.Permission,
                this.AccessFineLocation.Permission,],

            >= BuildVersionCodes.Q => [this.Bluetooth.Permission,
                this.BluetoothAdmin.Permission,
                this.BluetoothConnect.Permission,
                this.AccessWifiState.Permission,
                this.ChangeWifiState.Permission,
                this.NearbyWifiDevices.Permission,
                this.AccessCoarseLocation.Permission,
                this.AccessFineLocation.Permission,],

            _ => [this.Bluetooth.Permission,
                this.BluetoothAdmin.Permission,
                this.BluetoothConnect.Permission,
                this.AccessWifiState.Permission,
                this.ChangeWifiState.Permission,
                this.AccessCoarseLocation.Permission,
                this.AccessFineLocation.Permission,]
        }));
    }
}
