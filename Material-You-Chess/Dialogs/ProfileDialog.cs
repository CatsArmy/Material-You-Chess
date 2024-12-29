using Android.Content;
using Chess.Util.Logger;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;

namespace Chess.Dialogs;

public class ProfileDialog : IProfileDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public MainActivity App { get; set; }
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; }
    public ShapeableImageView? DialogProfilePicture { get; set; }
    public Button? EditProfilePicture { get; set; }
    public TextView? ThemeText { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextView? EditProfileUsername { get; set; }
    public FloatingActionButton? EditUsername { get; set; }
    public UsernameDialog? UsernameDialog { get; set; }
    public bool WasShown { get; set; } = false;

    private Android.Net.Uri? PhotoUri = null;
    public ProfileDialog(MainActivity App)
    {
        this.App = App;
        this.Builder = new MaterialAlertDialogBuilder(App);
        this.Builder.SetIcon(Resource.Drawable.outline_settings_account_box);
        this.Builder.SetTitle("Profile");
        this.Builder.SetView(Resource.Layout.profile_dialog);
        this.Builder.SetPositiveButton("Confirm", this.OnConfirm);
        this.Builder.SetNegativeButton("Cancel", this.OnCancel);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += this.OnShow;
        this.UsernameDialog = new UsernameDialog(App, this.OnUsernameChange);
        this.UserProfileChangeRequest = new UserProfileChangeRequest.Builder();
    }

    public void OnUsernameChange(string username)
    {
        this.UserProfileChangeRequest.SetDisplayName(username);
        this.EditProfileUsername!.Text = username;
    }

    public void OnSelectPhoto(Android.Net.Uri photoUri)
    {
        this.PhotoUri = photoUri;
        this.DialogProfilePicture?.SetImageURI(photoUri);
        this.DialogProfilePicture?.RequestLayout();
    }

    public void OnShow(object? sender, EventArgs args)
    {
        this.UserProfileChangeRequest = new UserProfileChangeRequest.Builder();
        this.DialogProfilePicture = this.Dialog.FindViewById<ShapeableImageView>(Resource.Id.ProfilePicture);
        this.EditProfilePicture = this.Dialog.FindViewById<Button>(Resource.Id.editProfilePicture);
        this.ThemeText = this.Dialog.FindViewById<TextView>(Resource.Id.ThemeText);
        this.ThemeToggle = this.Dialog.FindViewById<MaterialSwitch>(Resource.Id.EditTheme);
        this.EditProfileUsername = this.Dialog.FindViewById<TextView>(Resource.Id.UsernameText);
        this.EditUsername = this.Dialog.FindViewById<FloatingActionButton>(Resource.Id.EditUsername);
        this.ThemeToggle!.CheckedChange += this.ThemeChanged;
        this.ThemeToggle!.Checked = this.App.MaterialYouThemePreference;
        if (FirebaseAuth.Instance?.CurrentUser?.DisplayName == null || FirebaseAuth.Instance?.CurrentUser?.DisplayName == string.Empty)
            Log.Debug("Display name is missing???");

        this.EditProfileUsername!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        this.DialogProfilePicture!.SetImageURI(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
        if (!this.WasShown)
        {
            this.EditProfilePicture!.Click += this.App.OpenPhotoPicker;
            this.EditUsername!.Click += (sender, args) => this.UsernameDialog?.Dialog.Show();
            this.WasShown = true;
        }
    }

    private void ThemeChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        this.App.MaterialYouThemePreference = e.IsChecked;
    }

    public async void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        if (this.UserProfileChangeRequest.PhotoUri == null)
        {
            //TODO implement logic for clearing pfp
            if (true)
                this.UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
        }

        if (this.UserProfileChangeRequest.DisplayName == null)
        {
            this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
        }

        await FirebaseAuth.Instance!.CurrentUser!.UpdateProfileAsync(this.UserProfileChangeRequest.Build());
    }

    public void OnCancel(object? sender, DialogClickEventArgs args)
    {
        this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
        this.UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
    }
}
