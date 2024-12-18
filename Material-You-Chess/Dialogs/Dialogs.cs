using Android.Content;
using Android.Util;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public interface IMaterialDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }

    public void OnShow(object sender, EventArgs args);
    public void OnConfirm(object sender, DialogClickEventArgs args);
    public void OnCancel(object sender, DialogClickEventArgs args);
}

public interface IProfileDialog : IMaterialDialog
{
    public AppCompatActivity App { get; set; }
    public Action UpdateInformation { get; set; }
    public Action<object, EventArgs> OpenPhotoPicker { get; set; }
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; }
    public ShapeableImageView? DialogProfilePicture { get; set; }
    public Button? EditProfilePicture { get; set; }
    public TextView? ThemeText { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextView? EditProfileUsername { get; set; }
    public FloatingActionButton? EditUsername { get; set; }
    public bool WasShown { get; set; }
}

public class ProfileDialog : IProfileDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public AppCompatActivity App { get; set; }
    public Action UpdateInformation { get; set; }
    public Action<object, EventArgs> OpenPhotoPicker { get; set; }
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; }
    public ShapeableImageView? DialogProfilePicture { get; set; }
    public Button? EditProfilePicture { get; set; }
    public TextView? ThemeText { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextView? EditProfileUsername { get; set; }
    public FloatingActionButton? EditUsername { get; set; }
    public bool WasShown { get; set; } = false;

    public ProfileDialog(AppCompatActivity App, Action<object, EventArgs> OpenPhotoPicker, Action UpdateInformation)
    {
        this.App = App;
        Builder = new MaterialAlertDialogBuilder(App, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetIcon(App.Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, Builder.Context.Theme));
        Builder.SetTitle("Profile");
        Builder.SetView(Resource.Layout.profile_dialog);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        this.Dialog = Builder.Create();
        this.Dialog.ShowEvent += OnShow;
        this.OpenPhotoPicker = OpenPhotoPicker;
        this.UpdateInformation = UpdateInformation;
    }

    public void OnShow(object sender, EventArgs args)
    {
        UserProfileChangeRequest = new UserProfileChangeRequest.Builder();
        this.DialogProfilePicture = this.Dialog.FindViewById<ShapeableImageView>(Resource.Id.ProfilePicture);
        this.EditProfilePicture = this.Dialog.FindViewById<Button>(Resource.Id.editProfilePicture);
        this.ThemeText = this.Dialog.FindViewById<TextView>(Resource.Id.ThemeText);
        this.ThemeToggle = this.Dialog.FindViewById<MaterialSwitch>(Resource.Id.EditTheme);
        this.EditProfileUsername = this.Dialog.FindViewById<TextView>(Resource.Id.UsernameText);
        this.EditUsername = this.Dialog.FindViewById<FloatingActionButton>(Resource.Id.EditUsername);

        this.DialogProfilePicture.SetImageURI(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
        this.EditProfilePicture.Click += (sender, args) => OpenPhotoPicker(sender, args);
        //this.EditUsername.Click += (sender, e) => editUsernameDialog.Show();

        //MaterialAlertDialogBuilder editUsernameDialogBuilder = new MaterialAlertDialogBuilder(App,
        //Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        //editUsernameDialogBuilder.SetTitle("Edit Username");
        //editUsernameDialogBuilder.SetView(Resource.Layout.username_dialog);
        //editUsernameDialogBuilder.SetPositiveButton("Confirm", (sender, args) =>
        //{
        //    editProfile.SetDisplayName(UsernameTextInput.Text);
        //    this.EditProfileUsername.Text = editProfile.DisplayName;
        //});
        //var editUsernameDialog = editUsernameDialogBuilder.Create();
        //editUsernameDialog.ShowEvent += (sender, e) =>
        //{
        //    UsernameTextInput = editUsernameDialog.FindViewById<TextInputEditText>(Resource.Id.MainUsernameInput);
        //    UsernameTextInput.Hint = FirebaseAuth.Instance.CurrentUser.DisplayName;
        //};
    }

    public async void OnConfirm(object sender, DialogClickEventArgs args)
    {
        if (UserProfileChangeRequest.PhotoUri is null && UserProfileChangeRequest.DisplayName is null)
            return;

        if (UserProfileChangeRequest.PhotoUri == null)
        {
            if (true)
                UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
        }

        if (UserProfileChangeRequest.DisplayName == null)
        {
            this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance.CurrentUser.DisplayName);
        }

        await FirebaseAuth.Instance.CurrentUser.UpdateProfileAsync(UserProfileChangeRequest.Build());
    }

    public void OnCancel(object sender, DialogClickEventArgs args)
    {
        UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance.CurrentUser.DisplayName);
        UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
    }
}

public interface ILoginDialog : IMaterialDialog
{
    public bool WasShown { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public TextInputEditText EmailInput { get; set; }
    public TextInputEditText PasswordInput { get; set; }
    public Action OnSuccess { get; set; }

}

public class LoginDialog : ILoginDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public bool WasShown { get; set; } = false;
    public string Email { get; set; }
    public string Password { get; set; }
    public TextInputEditText EmailInput { get; set; }
    public TextInputEditText PasswordInput { get; set; }
    public Action OnSuccess { get; set; }

    public LoginDialog(AppCompatActivity app, Action OnLoginSuccess)
    {
        Builder = new MaterialAlertDialogBuilder(app, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetIcon(app.Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, Builder.Context.Theme));
        Builder.SetTitle("Login");
        Builder.SetView(Resource.Layout.login);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.OnSuccess = OnLoginSuccess;
    }

    public void OnShow(object sender, EventArgs args)
    {
        this.EmailInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.LoginEmailInput);
        this.PasswordInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.LoginPasswordInput);
        if (!WasShown)
        {
            this.EmailInput.TextChanged += (sender, args) => this.Email = this.EmailInput.Text;
            this.PasswordInput.TextChanged += (sender, args) => this.Password = this.PasswordInput.Text;
            this.WasShown = true;
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
            this.OnSuccess();
        }
    }

    public void OnCancel(object sender, DialogClickEventArgs args)
    {
        this.EmailInput.Text = "";
        this.PasswordInput.Text = "";
    }

    public interface ISignupDialog : ILoginDialog
    {
        public string Username { get; set; }
        public TextInputEditText UsernameInput { get; set; }
        public TextInputLayout UsernameLayout { get; set; }
    }
}
