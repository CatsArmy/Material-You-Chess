using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public partial class SignupDialog : ISignupDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public bool WasShown { get; set; } = false;
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public TextInputEditText UsernameInput { get; set; }
    public TextInputLayout UsernameLayout { get; set; }
    public TextInputEditText EmailInput { get; set; }
    public TextInputLayout EmailLayout { get; set; }
    public TextInputLayout PasswordLayout { get; set; }
    public TextInputEditText PasswordInput { get; set; }
    public Action OnSuccess { get; set; }
    private AppCompatActivity App { get; set; }
    private bool hasCatched { get; set; } = false;

    public SignupDialog(AppCompatActivity App, Action OnLoginSuccess)
    {
        this.App = this.App;
        Builder = new MaterialAlertDialogBuilder(App, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetIcon(App.Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, Builder.Context.Theme));
        Builder.SetTitle("Login");
        Builder.SetView(Resource.Layout.login_signup_dialog);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.OnSuccess = OnLoginSuccess;
    }

    public void OnShow(object sender, EventArgs args)
    {
        this.UsernameInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.DialogUsernameInput);
        this.UsernameLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.DialogUsernameLayout);
        this.EmailInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.DialogEmailInput);
        this.EmailLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.DialogEmailLayout);
        this.PasswordLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.DialogPasswordLayout);
        this.PasswordInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.DialogPasswordInput);
        if (!WasShown)
        {
            this.UsernameInput.TextChanged += (sender, args) => this.Username = this.UsernameInput.Text;
            this.EmailInput.TextChanged += (sender, args) => this.Email = this.EmailInput.Text;
            this.PasswordInput.TextChanged += (sender, args) => this.Password = this.PasswordInput.Text;
            this.WasShown = true;
        }
        UsernameLayout.Visibility = ViewStates.Visible;
    }

    public async void OnConfirm(object sender, DialogClickEventArgs args)
    {
        if (Username == string.Empty)
        {
            if (Dialog.IsShowing)
                Dialog.Hide();
            Dialog.Show();
            UsernameLayout.Error = "Must fill username field";
            Toast.MakeText(Dialog.Context, "Authentication Error, missing field: Username", ToastLength.Long).Show();
            return;
        }

        try
        {
            this.hasCatched = false;
            var result = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(Email, Password);

        }
        catch (Exception e)
        {
            this.hasCatched = true;
            Log.Debug("CatDebug", $"{e}");
            if (Dialog.IsShowing)
                Dialog.Hide();
            Dialog.Show();

            //thrown if the user account corresponding to email does not exist or has been disabled
            if (e is FirebaseAuthInvalidUserException)
            {
                EmailLayout.Error = "The account corresponding to the email does not exist or has been disabled";
                Toast.MakeText(Dialog.Context, "Authentication Error", ToastLength.Long).Show();
                return;
            }
            //When Email Enumeration Protection is disabled,
            //throwns if the password is wrong
            //When Email Enumeration Protection is enabled (reccomended),
            //throwns if the email or password is invalid.
            if (e is FirebaseAuthInvalidCredentialsException)
            {
                const string error = "the email or password is invalid";
                EmailLayout.Error = error;
                PasswordLayout.Error = error;
            }
        }
        finally
        {
            if (!hasCatched)
            {
                UserProfileChangeRequest.Builder builder = new();
                builder.SetDisplayName(Username);
                //await FirebaseAuth.Instance.CurrentUser.ReloadAsync();
                await FirebaseAuth.Instance.CurrentUser.UpdateProfileAsync(builder.Build());
                //await FirebaseAuth.Instance.CurrentUser.ReloadAsync();
                this.OnSuccess();
            }
        }
    }

    public void OnCancel(object sender, DialogClickEventArgs args)
    {
        this.EmailInput.Text = "";
        this.PasswordInput.Text = "";
    }
}


