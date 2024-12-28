using Android.Content;
using Android.Util;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public class LoginDialog : ILoginDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public bool WasShown { get; set; } = false;
    public string? Email { get; set; }
    public string? Password { get; set; }
    public TextInputEditText? EmailInput { get; set; }
    public TextInputLayout? EmailLayout { get; set; }
    public TextInputLayout? PasswordLayout { get; set; }
    public TextInputEditText? PasswordInput { get; set; }
    public Action OnSuccess { get; set; }
    private MainActivity App { get; set; }
    private bool HasCaught { get; set; } = false;
    public LoginDialog(MainActivity App, Action OnLoginSuccess)
    {
        this.App = App;
        this.Builder = new MaterialAlertDialogBuilder(App);
        this.Builder.SetIcon(App.GetDrawable(Resource.Drawable.outline_person));
        this.Builder.SetTitle("Login");
        this.Builder.SetView(Resource.Layout.login_signup_dialog);
        this.Builder.SetPositiveButton("Confirm", OnConfirm);
        this.Builder.SetNegativeButton("Cancel", OnCancel);
        this.Dialog = Builder.Create();
        this.Dialog.ShowEvent += OnShow;
        this.OnSuccess = OnLoginSuccess;
    }

    public void OnShow(object? sender, EventArgs args)
    {
        this.EmailInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.DialogEmailInput);
        this.EmailLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.DialogEmailLayout);
        this.PasswordLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.DialogPasswordLayout);
        this.PasswordInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.DialogPasswordInput);
        if (!this.WasShown)
        {
            this.EmailInput!.TextChanged += (sender, args) => this.Email = this.EmailInput?.Text;
            this.PasswordInput!.TextChanged += (sender, args) => this.Password = this.PasswordInput?.Text;
            this.WasShown = true;
        }
    }

    public async void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        try
        {
            this.HasCaught = false;
            var result = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(this.Email!, this.Password!);
        }
        catch (Exception e)
        {
            this.HasCaught = true;
            Log.Debug("CatDebug", $"{e}");
            if (this.Dialog.IsShowing)
                this.Dialog.Hide();
            this.Dialog.Show();

            //thrown if the user account corresponding to email does not exist or has been disabled
            if (e is FirebaseAuthInvalidUserException)
            {
                this.EmailLayout!.Error = "The account corresponding to the email does not exist or has been disabled";
                Toast.MakeText(this.Dialog.Context, "Authentication Error", ToastLength.Long)?.Show();
                return;
            }
            //When Email Enumeration Protection is enabled
            //throwns if the email or password is invalid.
            if (e is FirebaseAuthInvalidCredentialsException)
            {
                const string error = "the email or password is invalid";
                this.EmailLayout!.Error = error;
                this.PasswordLayout!.Error = error;
            }
        }
        finally
        {
            if (!this.HasCaught)
                this.OnSuccess();
            this.App.StartProgressIndicator();
            await FirebaseAuth.Instance!.CurrentUser!.ReloadAsync();
            this.App.StopProgressIndicator();
        }
    }

    public void OnCancel(object? sender, DialogClickEventArgs args)
    {
        this.EmailInput!.Text = string.Empty;
        this.PasswordInput!.Text = string.Empty;
    }
}
