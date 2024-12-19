using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Util;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Chess.Dialogs;
using Chess.FirebaseApp;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private bool MaterialYouThemePreference = true;
    private PickVisualMediaRequest.Builder pickVisualMediaRequestBuilder;
    private ShapeableImageView MainProfileImageView;
    private ActivityResultLauncher photoPicker;
    private AppPermissions permissions;
    private Android.Net.Uri uri;
    private Button StartGame;
    private TextView MainUsername;
    private ExtendedFloatingActionButton profileAction1;
    private ExtendedFloatingActionButton profileAction2;
    private ProfileDialog profileDialog;
    private LogoutDialog logoutDialog;
    private LoginDialog loginDialog;
    private SignupDialog signupDialog;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);
        _ = new FirebaseSecrets();

        this.photoPicker = RegisterForActivityResult(new PickVisualMedia(),
    new ActivityResultCallback<Android.Net.Uri>(async uri =>
        {
            Log.Debug("PhotoPicker", $"{uri}");
            if (FirebaseAuth.Instance.CurrentUser == null)
                return;

            if (uri == null)
                return;
            base.ContentResolver?.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission);
            this.uri = uri;
            this.profileDialog.OnSelectPhoto(uri);
        }));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
            .SetMediaType(PickVisualMedia.ImageOnly.Instance);

        if (!this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
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
        this.MainUsername = base.FindViewById<TextView>(Resource.Id.MainUsername);
        this.profileAction1 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        this.logoutDialog = new LogoutDialog(this, UpdateUserState);
        this.loginDialog = new LoginDialog(this, UpdateUserState);
        this.signupDialog = new SignupDialog(this, UpdateUserState);
        this.profileDialog = new ProfileDialog(this, OpenPhotoPicker, UpdateUserState);
        UpdateUserState();
    }

    private void OpenPhotoPicker(object sender, EventArgs args) =>
        this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());

    ///Todo add confirmation
    private void OpenLogoutDialog(object sender, EventArgs e) => logoutDialog.Dialog.Show();
    private void OpenProfileDialog(object sender, EventArgs e) => profileDialog.Dialog.Show();
    private void OpenLoginDialog(object sender, EventArgs e) => loginDialog.Dialog.Show();
    private void OpenSignupDialog(object sender, EventArgs e) => signupDialog.Dialog.Show();

    private void UpdateUserState()
    {
        bool isLoggedIn = FirebaseAuth.Instance.CurrentUser != null;
        //Todo Add a progress indicator
        switch (isLoggedIn)
        {
            case true:
                this.profileAction1.Text = "Profile";
                this.profileAction1.Click -= OpenLoginDialog;
                this.profileAction1.Click += OpenProfileDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);

                this.profileAction2.Text = "Log out";
                this.profileAction2.Click -= OpenSignupDialog;
                this.profileAction2.Click += OpenLogoutDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);

                this.MainUsername.Text = FirebaseAuth.Instance.CurrentUser.DisplayName;
                this.MainProfileImageView.SetImageURI(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
                this.MainProfileImageView.RequestLayout();
                break;

            case false:
                this.profileAction1.Text = "Login";
                this.profileAction1.Click -= OpenProfileDialog;
                this.profileAction1.Click += OpenLoginDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);

                this.profileAction2.Text = "Sign up";
                this.profileAction2.Click -= OpenLogoutDialog;
                this.profileAction2.Click += OpenSignupDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_add);

                this.MainUsername.Text = "Guest";
                this.MainProfileImageView.SetImageURI(null);
                this.MainProfileImageView.RequestLayout();
                break;
        }
    }

    private void StartGame_Click(object sender, EventArgs e)
    {
        Intent intent = new Intent(this, typeof(ChessActivity))
        .PutExtra(nameof(ChessActivity.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}");
        base.StartActivity(intent);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}