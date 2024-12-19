using Android.Content;
using AndroidX.AppCompat.App;
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
    public UsernameDialog? UsernameDialog { get; set; }
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
        this.UsernameDialog = new UsernameDialog(App, OnUsernameChange);
    }

    public void OnUsernameChange(string username)
    {
        UserProfileChangeRequest.SetDisplayName(username);
        EditProfileUsername.Text = username;
    }
    public void OnSelectPhoto(Android.Net.Uri photoUri)
    {
        this.DialogProfilePicture?.SetImageURI(photoUri);
        this.DialogProfilePicture?.RequestLayout();
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

        if (FirebaseAuth.Instance.CurrentUser.DisplayName == null
            || FirebaseAuth.Instance.CurrentUser.DisplayName == string.Empty)
            Log.Debug("Display name is missing???");
        this.EditProfileUsername.Text = FirebaseAuth.Instance.CurrentUser.DisplayName;
        this.DialogProfilePicture.SetImageURI(FirebaseAuth.Instance.CurrentUser.PhotoUrl);
        if (!WasShown)
        {
            this.EditProfilePicture.Click += (sender, args) => OpenPhotoPicker(sender, args);
            this.EditUsername.Click += (sender, args) => this.UsernameDialog?.Dialog.Show();
            WasShown = true;
        }
    }

    public async void OnConfirm(object sender, DialogClickEventArgs args)
    {
        if (UserProfileChangeRequest.PhotoUri is null && UserProfileChangeRequest.DisplayName is null)
            return;

        if (UserProfileChangeRequest.PhotoUri == null)
        {
            if (true)//TODO implement logic for clearing pfp
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
