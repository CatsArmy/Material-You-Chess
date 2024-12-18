using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace Chess.Util;

public class AppPermissions
{
    private const BuildVersionCodes UpsideDownCake = BuildVersionCodes.Tiramisu + 1;
    public Permission READ_MEDIA_VISUAL_USER_SELECTED;
    public Permission READ_MEDIA_IMAGES;
    public Permission READ_MEDIA_VIDEO;
    public Permission READ_EXTERNAL_STORAGE;

    public AppPermissions()
    {
        this.READ_MEDIA_VISUAL_USER_SELECTED = Permission.Denied;
        this.READ_MEDIA_IMAGES = Permission.Denied;
        this.READ_MEDIA_VIDEO = Permission.Denied;
        this.READ_EXTERNAL_STORAGE = Permission.Denied;
    }

    public void RequestPermissions(AppCompatActivity app)
    {
        var (permissions, requestCode) = PermissionRequestLogic();
        app.RequestPermissions(permissions, requestCode);

        /*
        var requestPermissions = RegisterForActivityResult(new RequestMultiplePermissions(),
           new ActivityResultCallback<Java.Lang.Object>(results =>
           {
               // Handle permission requests results
               if (Build.VERSION.SdkInt >= (UpsideDownCake)
                   && app.CheckSelfPermission(nameof(READ_MEDIA_VISUAL_USER_SELECTED)) == Permission.Granted)
               {
                   // Partial access on Android 14 (API level 34) or higher
                   READ_MEDIA_VISUAL_USER_SELECTED = Permission.Granted;
                   READ_MEDIA_IMAGES = app.CheckSelfPermission(nameof(READ_MEDIA_IMAGES));
                   READ_MEDIA_VIDEO = app.CheckSelfPermission(nameof(READ_MEDIA_VIDEO));
               }
               else if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && app.CheckSelfPermission(nameof(READ_MEDIA_IMAGES)) == Permission.Granted)
               {
                   // Full access on Android 13 (API level 33) or higher
                   READ_MEDIA_IMAGES = Permission.Granted;
                   READ_MEDIA_VIDEO = app.CheckSelfPermission(nameof(READ_MEDIA_VIDEO));
               }
               else if (app.CheckSelfPermission(nameof(READ_EXTERNAL_STORAGE)) == Permission.Granted)
               {
                   // Full access up to Android 12 (API level 32)
                   READ_EXTERNAL_STORAGE = Permission.Granted;
               }
               Log.Debug("CatsArmy", $"{results}");
           }));
        */
    }

    public void HandlePermissionRequestsResults(AppCompatActivity app)
    {
        if (Build.VERSION.SdkInt >= (UpsideDownCake)) // Partial access on Android 14 (API level 34) or higher
        {
            READ_MEDIA_VISUAL_USER_SELECTED = app.CheckSelfPermission(nameof(READ_MEDIA_VISUAL_USER_SELECTED));
            READ_MEDIA_IMAGES = app.CheckSelfPermission(nameof(READ_MEDIA_IMAGES));
            READ_MEDIA_VIDEO = app.CheckSelfPermission(nameof(READ_MEDIA_VIDEO));
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Full access on Android 13 (API level 33) or higher
        {
            READ_MEDIA_IMAGES = app.CheckSelfPermission(nameof(READ_MEDIA_IMAGES));
            READ_MEDIA_VIDEO = app.CheckSelfPermission(nameof(READ_MEDIA_VIDEO));
        }
        else // Full access up to Android 12 (API level 32)
        {
            READ_EXTERNAL_STORAGE = app.CheckSelfPermission(nameof(READ_EXTERNAL_STORAGE));
        }
    }

    private (string[], int) PermissionRequestLogic()
    {
        if (Build.VERSION.SdkInt >= (UpsideDownCake))
            return (new string[] { nameof(READ_MEDIA_IMAGES), nameof(READ_MEDIA_VIDEO), nameof(READ_MEDIA_VISUAL_USER_SELECTED) }, 3);
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            return (new string[] { nameof(READ_MEDIA_IMAGES), nameof(READ_MEDIA_VIDEO) }, 2);
        else
            return (new string[] { nameof(READ_EXTERNAL_STORAGE) }, 1);
    }
}