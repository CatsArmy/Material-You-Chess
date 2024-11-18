using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Chess;


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
    private AndroidX.AppCompat.App.AlertDialog dialog;

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
                    this.MainProfileImageView.SetImageURI(uri);

                var user = await chessFirebase.auth.CreateUserWithEmailAndPasswordAsync("bergman.itai@gmail.com", "12345");
                await user.User.UpdateProfileAsync(new UserProfileChangeRequest.Builder()
                    .SetDisplayName("Guest3")
                    .SetPhotoUri(uri)
                    .Build());

                chessFirebase.auth.UpdateCurrentUser(user.User);

                //chessFirebase.auth.CurrentUser
            }));
        //chessFirebase.auth.ConfirmPasswordResetAsync
        //chessFirebase.auth.SendPasswordResetEmailAsync
        //chessFirebase.auth.VerifyPasswordResetCodeAsync


        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder()
            .SetMediaType(PickVisualMedia.ImageOnly.Instance);

        if (!this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);
        // Permission request logic
        this.permissions = new AppPermissions();
        this.permissions.RequestPermissions(this);
        //Run our logic
        this.StartGame = base.FindViewById<Button>(Resource.Id.btnStartGame);
        this.StartGame.Click += StartGame_Click;
        this.MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        //this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());
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