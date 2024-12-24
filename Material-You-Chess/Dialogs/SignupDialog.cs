﻿using Android.Content;
using Android.Util;
using Android.Views;
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
    private MainActivity App { get; set; }
    private bool HasCaught { get; set; } = false;

    public SignupDialog(MainActivity App, Action OnLoginSuccess)
    {
        this.App = App;
        Builder = new MaterialAlertDialogBuilder(App);
        Builder.SetIcon(Resource.Drawable.outline_person_add);
        Builder.SetTitle("Signup");
        Builder.SetView(Resource.Layout.login_signup_dialog);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.OnSuccess = OnLoginSuccess;
    }

    public void OnShow(object? sender, EventArgs args)
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

    public async void OnConfirm(object? sender, DialogClickEventArgs args)
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
            this.HasCaught = false;
            var result = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(Email, Password);

        }
        catch (Exception e)
        {
            this.HasCaught = true;
            Log.Debug("CatDebug", $"{e}");
            if (Dialog.IsShowing)
                Dialog.Hide();
            Dialog.Show();
            //thrown if the password is not strong enough
            if (e is FirebaseAuthWeakPasswordException)
            {
                PasswordLayout.Error = "Password too weak";
                Toast.MakeText(Dialog.Context, "Authentication Error", ToastLength.Long).Show();
            }
            //thrown if the email address is malformed
            if (e is FirebaseAuthInvalidCredentialsException)
            {
                EmailLayout.Error = "Email address is malformed";
                Toast.MakeText(Dialog.Context, "Authentication Error", ToastLength.Long).Show();
                return;
            }
            //thrown if there already exists an account with the given email address
            if (e is FirebaseAuthUserCollisionException)
            {
                EmailLayout.Error = "Account with the given email address already exists";
            }
        }
        finally
        {
            if (!HasCaught)
            {
                UserProfileChangeRequest.Builder builder = new();
                builder.SetDisplayName(Username);
                App.StartProgressIndicator();
                await FirebaseAuth.Instance?.CurrentUser?.UpdateProfileAsync(builder.Build());
                await FirebaseAuth.Instance?.CurrentUser?.ReloadAsync();
                App.StopProgressIndicator();
                this.OnSuccess();
            }
        }
    }

    public void OnCancel(object? sender, DialogClickEventArgs args)
    {
        this.EmailInput.Text = "";
        this.PasswordInput.Text = "";
    }
}


