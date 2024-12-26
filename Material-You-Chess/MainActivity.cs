using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Util;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Chess.Dialogs;
using Chess.FirebaseApp;
using Chess.Util;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Microsoft.Maui.ApplicationModel;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    private bool MaterialYouThemePreference = true;
    private ActivityResultLauncher? photoPicker;
    private AppPermissions? permissions;
    private Android.Net.Uri? selectedPhoto;
    private PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;

    private ShapeableImageView? mainProfilePicture;
    private Button? startGame;
    private TextView? mainUsername;
    private CircularProgressIndicator? UserProgressIndicator;
    private ExtendedFloatingActionButton? profileAction1;
    private ExtendedFloatingActionButton? profileAction2;
    private ProfileDialog? profileDialog;
    private LogoutDialog? logoutDialog;
    private LoginDialog? loginDialog;
    private SignupDialog? signupDialog;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);
        _ = new FirebaseSecrets();

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(), new ActivityResultCallback<Android.Net.Uri>(SelectPhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
            .SetMediaType(PickVisualMedia.ImageOnly.Instance);

        if (!this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);
        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);
        //base.StartActivity(new Intent(this, typeof(MainActivity2)));
        //return;

        // Permission request logic
        this.permissions = new AppPermissions(this);

        //Run our logic
        this.startGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
        this.UserProgressIndicator = base.FindViewById<CircularProgressIndicator>(Resource.Id.UserProgressIndicator);
        this.mainProfilePicture = base.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        this.mainUsername = base.FindViewById<TextView>(Resource.Id.MainUsername);
        this.profileAction1 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = base.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        this.logoutDialog = new LogoutDialog(this, UpdateUserState);
        this.loginDialog = new LoginDialog(this, UpdateUserState);
        this.signupDialog = new SignupDialog(this, UpdateUserState);
        this.profileDialog = new ProfileDialog(this, OpenPhotoPicker, UpdateUserState);
        this.startGame.Click += StartGame_Click;
        UpdateUserState();
    }

    private async void SelectPhoto(Android.Net.Uri photo)
    {
        Log.Debug("PhotoPicker", $"{photo}");
        if (FirebaseAuth.Instance.CurrentUser == null || photo == null)
            return;

        base.ContentResolver?.TakePersistableUriPermission(photo, ActivityFlags.GrantReadUriPermission);
        this.selectedPhoto = photo;
        this.profileDialog?.OnSelectPhoto(photo);
    }

    private void OpenPhotoPicker(object? sender, EventArgs args) => this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());
    private void OpenLogoutDialog(object? sender, EventArgs e) => logoutDialog?.Dialog.Show();
    private void OpenProfileDialog(object? sender, EventArgs e) => profileDialog?.Dialog.Show();
    private void OpenLoginDialog(object? sender, EventArgs e) => loginDialog?.Dialog.Show();
    private void OpenSignupDialog(object? sender, EventArgs e) => signupDialog?.Dialog.Show();


    private void UpdateUserState()
    {
        //Todo Add a progress indicator
        switch (FirebaseAuth.Instance.CurrentUser != null)
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

                this.mainUsername.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
                this.mainProfilePicture.SetImageURI(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
                this.mainProfilePicture.RequestLayout();
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

                this.mainUsername.Text = "Guest";
                this.mainProfilePicture.SetImageURI(null);
                this.mainProfilePicture.RequestLayout();
                break;
        }
    }

    public void StartProgressIndicator() => UserProgressIndicator.Show();

    public void StopProgressIndicator() => UserProgressIndicator.Hide();

    private void StartGame_Click(object? sender, EventArgs e)
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