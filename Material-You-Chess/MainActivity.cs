using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Chess.FirebaseApp;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
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
    private ExtendedFloatingActionButton profileAction1;
    private ExtendedFloatingActionButton profileAction2;
    private ShapeableImageView DialogProfilePicture;
    private AndroidX.AppCompat.App.AlertDialog editProfileDialog;
    private AndroidX.AppCompat.App.AlertDialog loginDialog;
    private AndroidX.AppCompat.App.AlertDialog signUpDialog;
    private TextView MainUsername;
    private TextView EditProfileUsername;
    private Bitmap newPfp;

    private UserProfileChangeRequest.Builder editProfile;
    private TextInputEditText EmailInput;
    private TextInputEditText PasswordInput;
    private TextInputEditText UsernameTextInput;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        _ = this.GetMaterialYouThemePreference(out this.MaterialYouThemePreference);
        new FirebaseSecrets();
        this.photoPicker = RegisterForActivityResult(new PickVisualMedia(),
    new ActivityResultCallback<Android.Net.Uri>(async uri =>
        {
            Log.Debug("PhotoPicker", $"{uri}");
            if (FirebaseAuth.Instance.CurrentUser == null)
                return;

            if (uri == null)
                return;

            var stream = this.ApplicationContext.OpenFileOutput("temp", FileCreationMode.Private);

            this.uri = uri;
            this.DialogProfilePicture.SetImageURI(uri);
            DialogProfilePicture.RequestLayout();
            DialogProfilePicture.RefreshDrawableState();
            this.editProfile.SetPhotoUri(uri);
            this.newPfp = ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(ContentResolver, uri));

            if (!await newPfp.CompressAsync(Bitmap.CompressFormat.Png, quality: 77, stream))
            {
                Log.Debug("CatDebug", "failed to compress bitmap");
                return;
            }
            stream.Close();
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
        InitDialogs();
        UpdateUserState();
    }

    private void OpenProfileDialog(object sender, EventArgs e)
    {
        editProfileDialog.Show();
    }

    private void OpenLoginDialog(object sender, EventArgs e)
    {
        loginDialog.Show();
    }

    private void OpenSignupDialog(object sender, EventArgs e) { }

    ///Todo add confirmation
    private void OpenLogoutDialog(object sender, EventArgs e)
    {
        FirebaseAuth.Instance.SignOut();
        UpdateUserState();
    }

    private void UpdateUserState()
    {
        bool isLoggedIn = FirebaseAuth.Instance.CurrentUser != null;

        switch (isLoggedIn)
        {
            case true:
                this.profileAction1.Text = "Profile";
                this.profileAction1.Click -= OpenLoginDialog;
                //this.profileAction1.Click -= OpenProfileDialog;
                this.profileAction1.Click += OpenProfileDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);

                this.profileAction2.Text = "Log out";
                this.profileAction2.Click -= OpenSignupDialog;
                //this.profileAction2.Click -= OpenLogoutDialog;
                this.profileAction2.Click += OpenLogoutDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);
                break;

            case false:
                this.profileAction1.Text = "Login";
                this.profileAction1.Click -= OpenProfileDialog;
                //this.profileAction1.Click -= OpenLoginDialog;
                this.profileAction1.Click += OpenLoginDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);

                this.profileAction2.Text = "Sign up";
                this.profileAction2.Click -= OpenLogoutDialog;
                //this.profileAction2.Click -= OpenSignupDialog;
                this.profileAction2.Click += OpenSignupDialog;
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
        editProfileDialog.SetPositiveButton("Confirm", async (sender, args) =>
        {
            if (editProfile.PhotoUri != null)
            {
                //Todo Fix firebase storage requesting billing information and not letting me input the fucking INFORMATION so i can replace this patch
                var tempStream = ApplicationContext.OpenFileInput("temp");
                var outputStream = ApplicationContext.OpenFileOutput("ProfilePicture", FileCreationMode.Private);
                tempStream.CopyTo(outputStream);
                tempStream.Close();
                outputStream.Close();
                ApplicationContext.DeleteFile("temp");
                var uri = Android.Net.Uri.FromFile(ApplicationContext.GetFileStreamPath("ProfilePicture"));
                editProfile.SetPhotoUri(uri);
                this.MainProfileImageView.SetImageURI(editProfile.PhotoUri);
                this.MainProfileImageView.RefreshDrawableState();
                this.DialogProfilePicture.SetImageURI(editProfile.PhotoUri);
            }
            if (editProfile.DisplayName != null)
            {
                this.MainUsername.Text = editProfile.DisplayName;
                this.EditProfileUsername.Text = editProfile.DisplayName;
            }

            await FirebaseAuth.Instance.CurrentUser.UpdateProfileAsync(editProfile.Build());
        });
        editProfileDialog.SetNegativeButton("Cancel", (sender, args) =>
        {
            editProfile.SetDisplayName(FirebaseAuth.Instance.CurrentUser.DisplayName);
            editProfile.SetPhotoUri(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
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
            try
            {
                var result = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception e)
            {
                Log.Debug("CatDebug", $"{e}");
            }
            finally
            {
                UpdateUserState();
            }

        });
        loginDialog.SetNegativeButton("Cancel", (object sender, DialogClickEventArgs args) =>
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
        this.editProfile = new UserProfileChangeRequest.Builder();
        this.DialogProfilePicture = this.editProfileDialog.FindViewById<ShapeableImageView>(Resource.Id.ProfilePicture);
        //this.DialogProfilePicture.SetImageURI(chessFirebase.auth.CurrentUser.PhotoUrl);
        var editProfilePicture = this.editProfileDialog.FindViewById<Button>(Resource.Id.editProfilePicture);
        editProfilePicture.Click += (_, _) => this.photoPicker.Launch(this.pickVisualMediaRequestBuilder.Build());

        var ThemeText = this.editProfileDialog.FindViewById<TextView>(Resource.Id.ThemeText);
        var ThemeToggle = this.editProfileDialog.FindViewById<MaterialSwitch>(Resource.Id.EditTheme);

        this.EditProfileUsername = this.editProfileDialog.FindViewById<TextView>(Resource.Id.UsernameText);
        var EditUsername = this.editProfileDialog.FindViewById<FloatingActionButton>(Resource.Id.EditUsername);

        MaterialAlertDialogBuilder editUsernameDialogBuilder = new MaterialAlertDialogBuilder(this,
        Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        editUsernameDialogBuilder.SetTitle("Edit Username");
        editUsernameDialogBuilder.SetView(Resource.Layout.username_dialog);
        editUsernameDialogBuilder.SetPositiveButton("Confirm", (sender, args) =>
        {
            editProfile.SetDisplayName(UsernameTextInput.Text);
            this.EditProfileUsername.Text = editProfile.DisplayName;
        });
        var editUsernameDialog = editUsernameDialogBuilder.Create();
        editUsernameDialog.ShowEvent += (sender, e) =>
        {
            UsernameTextInput = editUsernameDialog.FindViewById<TextInputEditText>(Resource.Id.MainUsernameInput);
            UsernameTextInput.Hint = editProfile.DisplayName;
        };
        EditUsername.Click += (sender, e) => editUsernameDialog.Show();
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

public interface IMaterialDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }

    public void OnShow(object sender, EventArgs args);
    public void OnConfirm(object sender, DialogClickEventArgs args);
    public void OnCancel(object sender, DialogClickEventArgs args);

}
public interface ILoginDialog : IMaterialDialog
{
    public string Email { get; set; }
    public string Password { get; set; }
    public TextInputEditText EmailInput { get; set; }
    public TextInputEditText PasswordInput { get; set; }
}

public class LoginDialog : ILoginDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public TextInputEditText EmailInput { get; set; }
    public TextInputEditText PasswordInput { get; set; }

    private Action onLoginSuccess;
    private bool wasShown = false;

    public LoginDialog(AppCompatActivity app, Action onLoginSuccess)
    {
        Builder = new MaterialAlertDialogBuilder(app, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetIcon(app.Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, Builder.Context.Theme));
        Builder.SetTitle("Login");
        Builder.SetView(Resource.Layout.login);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.onLoginSuccess = onLoginSuccess;
    }

    public void OnShow(object sender, EventArgs args)
    {
        this.EmailInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.LoginEmailInput);
        this.PasswordInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.LoginPasswordInput);
        if (!wasShown)
        {
            this.EmailInput.TextChanged += (sender, args) => this.Email = this.EmailInput.Text;
            this.PasswordInput.TextChanged += (sender, args) => this.Password = this.PasswordInput.Text;
            wasShown = true;
        }
        this.EmailInput.Text = "";
        this.PasswordInput.Text = "";
    }

    public async void OnConfirm(object sender, DialogClickEventArgs args)
    {
        try
        {
            var result = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(Email, Password);
        }
        catch (Exception e)
        {
            Log.Debug("CatDebug", $"{e}");
        }
        finally
        {
            this.onLoginSuccess();
        }
    }

    public void OnCancel(object sender, DialogClickEventArgs args)
    {
        this.EmailInput.Text = "";
        this.PasswordInput.Text = "";
    }

}