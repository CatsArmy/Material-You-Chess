using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Chess.Firebase;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
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
    private AndroidX.AppCompat.App.AlertDialog editProfileDialog;
    private AndroidX.AppCompat.App.AlertDialog loginDialog;
    private AndroidX.AppCompat.App.AlertDialog signUpDialog;
    private UserProfileChangeRequest.Builder editProfile;
    private TextInputEditText EmailInput;
    private TextInputEditText PasswordInput;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        Thread.Sleep(TimeSpan.FromSeconds(5));
        _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);
        chessFirebase = new ChessFirebase();
        this.photoPicker = RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<Android.Net.Uri>(async uri =>
            {
                Log.Debug("PhotoPicker", $"{uri}");
                if (chessFirebase.auth.CurrentUser == null)
                    return;

                if (uri == null)
                {
                    return;
                }
                editProfile.SetPhotoUri(uri);
            }));


        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
            .SetMediaType(PickVisualMedia.ImageOnly.Instance);

        if (!this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);
        //base.StartActivity(new Intent(this, typeof(MainActivity2)));
        //return;

        // Permission request logic
        this.permissions = new AppPermissions();
        this.permissions.RequestPermissions(this);
        //Run our logic
        this.StartGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
        this.StartGame.Click += StartGame_Click;
        this.MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        this.profileAction1 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        InitDialogs();
        UpdateUserState();
    }

    private void UpdateUserState()
    {
        bool isLoggedIn = chessFirebase.auth.CurrentUser != null;

        switch (isLoggedIn)
        {
            case true:
                this.profileAction1.Text = "Profile";
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);
                this.profileAction1.Click += (sender, e) => editProfileDialog.Show();
                this.profileAction2.Text = "Log out";
                this.profileAction2.Click += (sender, e) =>
                {
                    chessFirebase.auth.SignOut();
                    UpdateUserState();
                };
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);
                break;

            case false:
                //TODO alter the dumb fucking
                this.profileAction1.Text = "Login";
                this.profileAction1.Click += (sender, e) =>
                {
                    loginDialog.Show();
                };
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);
                this.profileAction2.Text = "Sign up";
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_add);
                break;
        }
    }

    private void InitDialogs()
    {
        MaterialAlertDialogBuilder editProfileDialog = new MaterialAlertDialogBuilder(this,
            Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        editProfileDialog.SetIcon(Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, editProfileDialog.Context.Theme));
        editProfileDialog.SetTitle("Profile");
        editProfileDialog.SetView(Resource.Layout.profile_dialog);
        editProfileDialog.SetPositiveButton("Confirm", (sender, args) =>
        {
            if (editProfile.DisplayName == chessFirebase.auth.CurrentUser.DisplayName
            && editProfile.PhotoUri == chessFirebase.auth.CurrentUser.PhotoUrl)
            {
                return;
            }
            chessFirebase.auth.CurrentUser.UpdateProfileAsync(editProfile.Build());
        });
        editProfileDialog.SetNegativeButton("Cancel", (sender, args) =>
        {
            editProfile.SetDisplayName(chessFirebase.auth.CurrentUser.DisplayName);
            editProfile.SetPhotoUri(chessFirebase.auth.CurrentUser.PhotoUrl);
        });
        this.editProfileDialog = editProfileDialog.Create();
        this.editProfileDialog.ShowEvent += Dialog_ShowEvent;
        MaterialAlertDialogBuilder loginDialog = new MaterialAlertDialogBuilder(this,
            Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        loginDialog.SetIcon(Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, loginDialog.Context.Theme));
        loginDialog.SetTitle("Login");
        loginDialog.SetView(Resource.Layout.login);
        loginDialog.SetPositiveButton("Confirm", async (sender, args) =>
        {
            string email = this.EmailInput.Text;
            string password = this.PasswordInput.Text;
            var result = await chessFirebase.auth.SignInWithEmailAndPasswordAsync(email, password);
            if (result == null)
            {

            }
        });
        loginDialog.SetNegativeButton("Cancel", (sender, args) =>
        {
            this.EmailInput.Text = "";
            this.PasswordInput.Text = "";
        });
        this.loginDialog = loginDialog.Create();
        this.loginDialog.ShowEvent += (sender, e) =>
        {
            this.EmailInput = this.loginDialog.FindViewById<TextInputEditText>(Resource.Id.LoginEmailInput);
            this.PasswordInput = this.loginDialog.FindViewById<TextInputEditText>(Resource.Id.LoginPasswordInput);
        };
    }

    private void Dialog_ShowEvent(object sender, EventArgs e)
    {
        editProfile = new UserProfileChangeRequest.Builder();
        this.DialogProfilePicture = this.editProfileDialog.FindViewById<ShapeableImageView>(Resource.Id.ProfilePicture);
        var editProfilePicture = this.editProfileDialog.FindViewById<Button>(Resource.Id.editProfilePicture);
        editProfilePicture.Click += (_, _) => this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());

        var ThemeText = this.editProfileDialog.FindViewById<TextView>(Resource.Id.ThemeText);
        var ThemeToggle = this.editProfileDialog.FindViewById<MaterialSwitch>(Resource.Id.EditTheme);

        var UsernameText = this.editProfileDialog.FindViewById<TextView>(Resource.Id.UsernameText);
        var EditUsername = this.editProfileDialog.FindViewById<FloatingActionButton>(Resource.Id.EditUsername);


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