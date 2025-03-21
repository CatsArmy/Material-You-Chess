using System.Runtime.Intrinsics.X86;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Activity.Result;
using Chess.App.Common.ActivityResult;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using Request = Android.Manifest.Permission;

namespace Chess.App.Common;

/// <summary>
/// Request code format is:
/// 00-09: Camera || Misc
/// 10-19: Media Access
/// 20-29: Nearby Access
/// </summary>
public class PermissionsRequester
{
    /// <summary> Request Code: 09 </summary>
    public Permission Camera = Permission.Denied;

    /// <summary> Request Code: 10 </summary>
    public Permission ReadMediaVisualUserSelected = Permission.Denied;

    /// <summary> Request Code: 11 </summary>
    public Permission ReadMediaImages = Permission.Denied;
    /// <summary> Request Code: 12  </summary>

    public Permission ReadMediaVideo = Permission.Denied;

    /// <summary> Request Code: 13  </summary>
    public Permission ReadExternalStorage = Permission.Denied;


    /// <summary> Request Code: 20 </summary>
    public Permission NearbyDevices = Permission.Denied;

    /// <summary> Request Code: 21 </summary>
    public Permission AccessWifiState = Permission.Denied;

    /// <summary> Request Code: 22 </summary> 
    public Permission ChangeWifiState = Permission.Denied;

    /// <summary> Request Code: 23 </summary> 
    public Permission Bluetooth = Permission.Denied;

    /// <summary> Request Code: 24 </summary> 
    public Permission BluetoothAdmin = Permission.Denied;

    /// <summary> Request Code: 25 </summary> 
    public Permission BluetoothAdvertise = Permission.Denied;

    /// <summary> Request Code: 26 </summary> 
    public Permission BluetoothConnect = Permission.Denied;

    /// <summary> Request Code: 27 </summary>
    public Permission BluetoothScan = Permission.Denied;

    /// <summary> Request Code: 28 </summary>    
    public Permission CoarseLocationAccess = Permission.Denied;

    /// <summary> Request Code: 29 </summary>
    public Permission FineLocationAccess = Permission.Denied;

    public Activity Activity { get; }

    private Action? OnGrantNearbyAccess;
    private Action? OnGrantCameraAccess;
    private Action? OnGrantMediaAccess;
    private ActivityResultLauncher requestPermissionLauncher;

    public PermissionsRequester(Main_Activity activity) : this(activity as Activity)
    {
        requestPermissionLauncher = //<I>: string[], <O>: Dictionary<string, bool>>
        activity.RegisterForActivityResult(new RequestMultiplePermissions(),
        callback: new ActivityResultCallback<IMap<string, bool>>((permissions) =>
        {
            foreach (var isGranted in permissions!.Values().Cast<bool>().ToArray())
            {
                if (!isGranted)
                {
                    // Explain to the user that the feature is unavailable because the
                    // feature requires a permission that the user has denied. At the
                    // same time, respect the user's decision. Don't link to system
                    // settings in an effort to convince the user to change their decision.

                    return;
                }

                // Permission is granted. Continue the action or workflow in your app.
            }
        }));
    }

    public PermissionsRequester(Activity activity)
    {
        this.Activity = activity;
        this.Camera = activity.CheckSelfPermission(Request.Camera);
        try
        {
            // Partial access on Android 11 (API level 31) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.S)
            {
                this.AccessWifiState = activity.CheckSelfPermission(Request.AccessWifiState);
                this.ChangeWifiState = activity.CheckSelfPermission(Request.ChangeWifiState);
            }

            // Partial access on Android 10 (API level 30) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.R)
            {
                this.Bluetooth = activity.CheckSelfPermission(Request.Bluetooth);
                this.BluetoothAdmin = activity.CheckSelfPermission(Request.BluetoothAdmin);
            }

            // Partial access on Android 8 (API level 28) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
                this.CoarseLocationAccess = activity.CheckSelfPermission(Request.AccessCoarseLocation);

            // Partial access on Android 9 (API level 29) or higher or equal to Android 11 (API level 31)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q && Build.VERSION.SdkInt <= BuildVersionCodes.S)
                this.FineLocationAccess = activity.CheckSelfPermission(Request.AccessFineLocation);

            // Partial access on Android 11 (API level 31) or higher
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                this.BluetoothAdvertise = activity.CheckSelfPermission(Request.BluetoothAdvertise);
                this.BluetoothConnect = activity.CheckSelfPermission(Request.BluetoothConnect);
                this.BluetoothScan = activity.CheckSelfPermission(Request.BluetoothScan);
            }

            // Partial access on Android 13 (API level 33) or higher
#pragma warning disable CA1416 // Validate platform compatibility
            if (Build.VERSION.SdkInt >= BuildVersionCodes.SV2)  /* why is the analyzer still flagging this as CA1416  */
                this.NearbyDevices = activity.CheckSelfPermission(Request.NearbyWifiDevices);
#pragma warning restore CA1416 // Validate platform compatibility

            // Partial access on Android 14 (API level 34) or higher
#pragma warning disable CA1416 // Validate platform compatibility
            if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)  /* why is the analyzer still flagging this as CA1416  */
                this.ReadMediaVisualUserSelected = activity.CheckSelfPermission(Request.ReadMediaVisualUserSelected);
#pragma warning restore CA1416 // Validate platform compatibility

            // Full access on Android 13 (API level 33) or higher
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)  /* why is the analyzer still flagging this as CA1416  */
            {
#pragma warning disable CA1416 // Validate platform compatibility
                this.ReadMediaImages = activity.CheckSelfPermission(Request.ReadMediaImages);
                this.ReadMediaVideo = activity.CheckSelfPermission(Request.ReadMediaVideo);
#pragma warning restore CA1416 // Validate platform compatibility
                return;
            }

            // Full access up to Android 12 (API level 32)
            this.ReadExternalStorage = activity.CheckSelfPermission(Request.ReadExternalStorage);
        }
        catch (Exception) { }
    }

    public void RequestNearbyConnectionsAccess(Action onGranted)
    {
        this.OnGrantNearbyAccess = onGranted;
        try
        {
            // Partial access on Android 13 (API level 33) or higher
#pragma warning disable CA1416 // Validate platform compatibility
            if (Build.VERSION.SdkInt >= BuildVersionCodes.SV2)  /* why is the analyzer still flagging this as CA1416  */
                this.Activity.RequestPermissions([Request.NearbyWifiDevices], 20);
#pragma warning restore CA1416 // Validate platform compatibility

            // Partial access on Android 11 (API level 31) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.S)
            {
                this.Activity.RequestPermissions([Request.AccessWifiState], 21);
                this.Activity.RequestPermissions([Request.ChangeWifiState], 22);
            }

            // Partial access on Android 10 (API level 30) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.R)
            {
                this.Activity.RequestPermissions([Request.Bluetooth], 23);
                this.Activity.RequestPermissions([Request.BluetoothAdmin], 24);
            }

            // Partial access on Android 8 (API level 28) or lower
            if (Build.VERSION.SdkInt <= BuildVersionCodes.P)
                this.Activity.RequestPermissions([Request.AccessCoarseLocation], 28);

            // Partial access on Android 9 (API level 29) or higher or equal to Android 11 (API level 31)
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.Q && Build.VERSION.SdkInt <= BuildVersionCodes.S)
                this.Activity.RequestPermissions([Request.AccessFineLocation], 29);

            // Partial access on Android 11 (API level 31) or higher
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                this.Activity.RequestPermissions([Request.BluetoothAdvertise], 25);
                this.Activity.RequestPermissions([Request.BluetoothConnect], 26);
                this.Activity.RequestPermissions([Request.BluetoothScan], 27);
            }
        }
        catch (Exception) { }
    }

    public void RequestMediaAccess(Action onGranted)
    {
        this.OnGrantMediaAccess = onGranted;
        try
        {
            // Partial access on Android 14 (API level 34) or higher 
#pragma warning disable CA1416 // Validate platform compatibility
            if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)   /* why is the analyzer still flagging this as CA1416  */
                this.Activity.RequestPermissions([Request.ReadMediaVisualUserSelected], 10);
#pragma warning restore CA1416 // Validate platform compatibility


            // Full access on Android 13 (API level 33) or higher
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)   /* why is the analyzer still flagging this as CA1416  */
            {
#pragma warning disable CA1416 // Validate platform compatibility
                this.Activity.RequestPermissions([Request.ReadMediaVideo], 11);
                this.Activity.RequestPermissions([Request.ReadMediaImages], 12);
#pragma warning restore CA1416 // Validate platform compatibility
                return;
            }

            // Full access up to Android 12 (API level 32)
            this.Activity.RequestPermissions([Request.ReadExternalStorage], 13);
        }
        catch (Exception) { }
    }

    public void RequestCamaraAccess(Action onGranted)
    {
        this.OnGrantCameraAccess = onGranted;
        try
        {
            this.Activity.RequestPermissions([Request.Camera], 9);
        }
        catch (Exception) { }
    }

    public bool HasCameraAccess()
    {
        if (this.Camera == Permission.Granted)
        {
            return true;
        }

        return false;
    }

    public bool HasNearbyAccess()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            return this.BluetoothScan == Permission.Granted &&
                this.BluetoothAdvertise == Permission.Granted &&
                this.BluetoothConnect == Permission.Granted &&
                this.AccessWifiState == Permission.Granted &&
                this.ChangeWifiState == Permission.Granted &&
                this.NearbyDevices == Permission.Granted;
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            return this.BluetoothScan == Permission.Granted &&
                this.BluetoothAdvertise == Permission.Granted &&
                this.BluetoothConnect == Permission.Granted &&
                this.AccessWifiState == Permission.Granted &&
                this.ChangeWifiState == Permission.Granted &&
                this.NearbyDevices == Permission.Granted &&
                this.CoarseLocationAccess == Permission.Granted &&
                this.FineLocationAccess == Permission.Granted;
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            return this.Bluetooth == Permission.Granted &&
                this.BluetoothAdmin == Permission.Granted &&
                this.BluetoothConnect == Permission.Granted &&
                this.AccessWifiState == Permission.Granted &&
                this.ChangeWifiState == Permission.Granted &&
                this.NearbyDevices == Permission.Granted &&
                this.CoarseLocationAccess == Permission.Granted &&
                this.FineLocationAccess == Permission.Granted;
        }
        return this.Bluetooth == Permission.Granted &&
            this.BluetoothAdmin == Permission.Granted &&
            this.BluetoothConnect == Permission.Granted &&
            this.AccessWifiState == Permission.Granted &&
            this.ChangeWifiState == Permission.Granted &&
            this.CoarseLocationAccess == Permission.Granted &&
            this.FineLocationAccess == Permission.Granted;
    }

    public bool HasMediaAccess()
    {
        if (this.ReadMediaVisualUserSelected == Permission.Granted || this.ReadMediaImages == Permission.Granted
            && this.ReadMediaVideo == Permission.Granted || this.ReadExternalStorage == Permission.Granted)
        {
            return true;
        }

        return false;
    }

    public void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        switch (requestCode)
        {
            case 9:
                this.Camera = grantResults[0];
                if (this.Camera != Permission.Granted || this.OnGrantCameraAccess is null)
                    return;

                this.OnGrantCameraAccess();
                this.OnGrantCameraAccess = null;
                return;

            case 10:
                this.ReadMediaVisualUserSelected = grantResults[0];
                break;

            case 11:
                this.ReadMediaVideo = grantResults[0];
                break;

            case 12:
                this.ReadMediaImages = grantResults[0];
                break;

            case 13:
                this.ReadExternalStorage = grantResults[0];
                break;

            case 20:
                this.NearbyDevices = grantResults[0];
                break;

            case 21:
                this.AccessWifiState = grantResults[0];
                break;

            case 22:
                this.ChangeWifiState = grantResults[0];
                break;

            case 23:
                this.Bluetooth = grantResults[0];
                break;

            case 24:
                this.BluetoothAdmin = grantResults[0];
                break;
            case 25:
                this.BluetoothAdvertise = grantResults[0];
                break;

            case 26:
                this.BluetoothConnect = grantResults[0];
                break;

            case 27:
                this.BluetoothScan = grantResults[0];
                break;

            case 28:
                this.CoarseLocationAccess = grantResults[0];
                this.FineLocationAccess = this.Activity.CheckSelfPermission(Request.AccessFineLocation);
                break;

            case 29:
                this.FineLocationAccess = grantResults[0];
                this.CoarseLocationAccess = this.Activity.CheckSelfPermission(Request.AccessCoarseLocation);
                break;

            default:
                break;
        }


        if (requestCode >= 10 && requestCode < 20)
        {
            if (!this.HasMediaAccess() || this.OnGrantMediaAccess is null)
            {
                return;
            }

            this.OnGrantMediaAccess();
            this.OnGrantMediaAccess = null;
        }

        else if (requestCode >= 20 && requestCode < 30)
        {
            if (!this.HasNearbyAccess() || this.OnGrantNearbyAccess is null)
            {
                return;
            }

            this.OnGrantNearbyAccess();
            this.OnGrantNearbyAccess = null;
        }
    }
}
