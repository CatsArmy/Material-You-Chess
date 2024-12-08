using System;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Chess.Firebase;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess;


//[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
//public class MainActivity2 : AppCompatActivity
//{
//    public const int BluetoothRequestCode = 200;
//    private bool BluetoothEnabled = false;
//    protected override void OnCreate(Bundle savedInstanceState)
//    {
//        BluetoothManager bluetoothManager = base.GetSystemService(BluetoothService) as BluetoothManager;
//        var adapter = bluetoothManager.Adapter;
//        if (adapter == null || adapter.IsEnabled)
//        {
//            return;
//        }
//        Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
//        base.StartActivityForResult(enableBtIntent, BluetoothRequestCode);
//        adapter.StartDiscovery();
//        BroadcastReceiver enableBtReceiver = new BluetoothDiscoveryReceiver();
//        IntentFilter filter = new IntentFilter(BluetoothDevice.ActionFound);

//        var intent = RegisterReceiver(enableBtReceiver, filter);

//    }

//    protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
//    {
//        base.OnActivityResult(requestCode, resultCode, data);
//        if (requestCode == BluetoothRequestCode)
//        {
//            if (resultCode == Result.Ok)
//            {
//                BluetoothEnabled = true;
//            }
//            else
//            {
//                BluetoothEnabled = false;
//            }
//        }

//    }
//}
[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private string PlayerName = "Guest2";
    private bool MaterialYouThemePreference = true;
    private PickVisualMediaRequest.Builder pickVisualMediaRequestBuilder;
    private ShapeableImageView MainProfileImageView;
    private ActivityResultLauncher photoPicker;
    private AppPermissions permissions;
    private Android.Net.Uri uri;
    private Button StartGame;
    private ExtendedFloatingActionButton profileAction1;
    private ExtendedFloatingActionButton profileAction2;
    private ChessFirebase chessFirebase;
    private ShapeableImageView DialogProfilePicture;
    private AndroidX.AppCompat.App.AlertDialog dialog;
    private BluetoothDevice device;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);
        chessFirebase = new ChessFirebase();
        this.photoPicker = RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<Android.Net.Uri>(async uri =>
            {
                //Logic
                Log.Debug("PhotoPicker", $"{uri}");
                if (uri != null)
                {
                    this.MainProfileImageView.SetImageURI(uri);
                    this.DialogProfilePicture.SetImageURI(uri);
                }

                //var user = await chessFirebase.auth.CreateUserWithEmailAndPasswordAsync(chessFirebase.TemplateEmail, chessFirebase.TemplatePassword);
                var user = await chessFirebase.auth.SignInWithEmailAndPasswordAsync(chessFirebase.TemplateEmail, chessFirebase.TemplatePassword);
                await user.User.UpdateProfileAsync(new UserProfileChangeRequest.Builder()
                    //.SetDisplayName("Guest3")
                    //    .SetPhotoUri(uri)
                    .Build());

                chessFirebase.auth.UpdateCurrentUser(user.User);
            }));
        //chessFirebase.auth.ConfirmPasswordResetAsync
        //chessFirebase.auth.SendPasswordResetEmailAsync
        //chessFirebase.auth.VerifyPasswordResetCodeAsync


        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
            .SetMediaType(PickVisualMedia.ImageOnly.Instance);
        const int locationPermissionsRequestCode = 1000;

        var locationPermissions = (Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation);
        if (ContextCompat.CheckSelfPermission(this, locationPermissions.AccessFineLocation) == Permission.Denied ||
                ContextCompat.CheckSelfPermission(this, locationPermissions.AccessCoarseLocation) == Permission.Denied)
        {
            ActivityCompat.RequestPermissions(this, new string[] { locationPermissions.AccessFineLocation, locationPermissions.AccessCoarseLocation },
                locationPermissionsRequestCode);
        }
        var bluetooth = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth);
        var bluetoothConnect = ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect);
        var bluetoothScan = ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan);
        var bluetoothAdmin = ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdmin);
        var bluetoothAdvertise = ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdvertise);
        var nearbyWifiDevices = ContextCompat.CheckSelfPermission(this, Manifest.Permission.NearbyWifiDevices);

        if (!this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);
        //base.StartActivity(new Intent(this, typeof(MainActivity2)));
        return;
        //var deviceManager = new BluetoothDeviceManager(this);
        //deviceManager.PickDevice(device => this.device = device);

        // Permission request logic
        this.permissions = new AppPermissions();
        this.permissions.RequestPermissions(this);
        //Run our logic
        this.StartGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
        this.StartGame.Click += StartGame_Click;
        this.MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        bool isLoggedIn = true;
        this.profileAction1 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        MaterialAlertDialogBuilder dialog = new MaterialAlertDialogBuilder(this,
            Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, dialog.Context.Theme));
        dialog.SetTitle("Profile");
        dialog.SetView(Resource.Layout.profile_dialog);
        dialog.SetPositiveButton("Confirm", (sender, args) =>
        {

        });
        dialog.SetNegativeButton("Cancel", (sender, args) =>
        {

        });
        this.dialog = dialog.Create();
        this.dialog.ShowEvent += Dialog_ShowEvent;

        switch (isLoggedIn)
        {
            case true:
                this.profileAction1.Text = "Profile";
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);
                this.profileAction1.Click += ProfileAction1_Click;
                this.profileAction2.Text = "Log out";
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);
                break;

            case false:
                this.profileAction1.Text = "Login";
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);
                this.profileAction2.Text = "Sign up";
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_add);
                break;
        }

    }

    private void Dialog_ShowEvent(object sender, EventArgs e)
    {
        this.DialogProfilePicture = this.dialog.FindViewById<ShapeableImageView>(Resource.Id.ProfilePicture);
        var editProfilePicture = this.dialog.FindViewById<Button>(Resource.Id.editProfilePicture);
        editProfilePicture.Click += (_, _) => this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());

        var ThemeText = this.dialog.FindViewById<TextView>(Resource.Id.ThemeText);
        var ThemeToggle = this.dialog.FindViewById<MaterialSwitch>(Resource.Id.EditTheme);

        var UsernameText = this.dialog.FindViewById<TextView>(Resource.Id.UsernameText);
        var EditUsername = this.dialog.FindViewById<FloatingActionButton>(Resource.Id.EditUsername);

    }

    private void ProfileAction1_Click(object sender, EventArgs e)
    {
        this.dialog.Show();
    }

    private void StartGame_Click(object sender, EventArgs e)
    {
        Intent intent = new Intent(this, typeof(ChessActivity))
        .PutExtra(nameof(ChessActivity.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}")
        .PutExtra(nameof(this.PlayerName), this.PlayerName)
        .PutExtra(nameof(chessFirebase.auth.CurrentUser), chessFirebase.auth.CurrentUser)
        .PutExtra(nameof(this.uri), $"{this.uri}");
        base.StartActivity(intent);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}