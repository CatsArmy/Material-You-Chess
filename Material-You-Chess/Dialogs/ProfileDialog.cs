using Android.Content;
using Android.Gms.Extensions;
using Android.Graphics;
using Bumptech.Glide;
using Chess.Util.Logger;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public class ProfileDialog : IProfileDialog
{
    public AlertDialog Dialog { get; set; }
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
    public Bitmap? PhotoBitmap { get; set; } = null;
    public bool WasShown { get; set; } = false;

    public ProfileDialog(MainActivity app)
    {
        this.App = app;
        this.Builder = new MaterialAlertDialogBuilder(app);
        this.Builder.SetIcon(Resource.Drawable.outline_settings_account_box);
        this.Builder.SetTitle("Profile");
        this.Builder.SetView(Resource.Layout.profile_dialog);
        this.Builder.SetPositiveButton("Confirm", this.OnConfirm);
        this.Builder.SetNegativeButton("Cancel", this.OnCancel);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += this.OnShow;
        this.UsernameDialog = new UsernameDialog(app, this.OnUsernameChange);
        this.UserProfileChangeRequest = new UserProfileChangeRequest.Builder();
    }

    public void OnShow(object? sender, EventArgs args)
    {
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

        this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
        this.UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);

        this.EditProfileUsername!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;

        this.DialogProfilePicture!.SetImageURI(null);
        if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is Android.Net.Uri PhotoUrl)
            Glide.With(this.Dialog.Context).Load(FirebaseStorage.Instance.Reference.Child($"{PhotoUrl}")).Into(this.DialogProfilePicture!);
        if (!this.WasShown)
        {
            this.App.RegisterForContextMenu(this.DialogProfilePicture!);
            this.App.RegisterForContextMenu(this.EditProfilePicture!);

            this.EditUsername!.Click += (sender, args) => this.UsernameDialog?.Dialog.Show();
            this.DialogProfilePicture!.Click += this.App.OpenPhotoPicker;
            this.EditProfilePicture!.Click += this.App.OpenPhotoTaker;

            this.EditUsername.Clickable = true;
            this.DialogProfilePicture.Clickable = true;
            this.EditProfilePicture.Clickable = true;
            this.WasShown = true;
        }
    }

    public async void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        await OnConfirmProfilePicture();
        await OnConfirmProfileUsername();
    }

    public void OnCancel(object? sender, DialogClickEventArgs args)
    {
        this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
        this.UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
    }

    public void OnSelectPhoto(Bitmap photo)
    {
        this.PhotoBitmap = photo;
        this.DialogProfilePicture?.SetImageBitmap(photo);
        this.DialogProfilePicture?.RequestLayout();
    }

    public void OnClearPhoto()
    {
        this.PhotoBitmap = null;
        this.DialogProfilePicture!.SetImageURI(null);
        this.DialogProfilePicture?.RequestLayout();
    }

    public void OnUsernameChange(string username)
    {
        this.UserProfileChangeRequest.SetDisplayName(username);
        this.EditProfileUsername!.Text = username;
    }

    private void ThemeChanged(object? sender, CompoundButton.CheckedChangeEventArgs e) => this.App.MaterialYouThemePreference = e.IsChecked;

    public async Task<bool> OnConfirmProfilePicture()
    {
        StorageReference path = FirebaseStorage.Instance.Reference.Child((FirebaseAuth.Instance!.CurrentUser!.PhotoUrl != null) switch
        {
            true => $"{FirebaseAuth.Instance!.CurrentUser!.PhotoUrl}",
            false => $"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png",
        });

        if (this.PhotoBitmap == null)
        {
            var delete = path.DeleteAsync();
            await delete;
            if (delete.IsCompletedSuccessfully)
            {
                /* Todo handle delete success and inform user */
                this.UserProfileChangeRequest.SetPhotoUri(null);
                Glide.Get(this.App).ClearDiskCache();
                return true;
            }

            else if (delete.IsFaulted)
            { /* Todo handle delete failure */ }

            else if (delete.IsCanceled)
            { /* Todo handle delete cancelation */ }

            return false;
        }

        Task<UploadTask.TaskSnapshot>? upload;
        using var stream = new MemoryStream();
        if (await this.PhotoBitmap.CompressAsync(Bitmap.CompressFormat.Png!, 77, stream))
        {
            var data = stream.ToArray();
            upload = path.PutBytes(data).AsAsync<UploadTask.TaskSnapshot>();
        }

        /* Todo handle upload failure */
        else
            return false;

        var snapshot = await upload!;
        if (upload.IsCompletedSuccessfully)
        {
            Glide.Get(this.App).ClearDiskCache();
            Glide.With(this.App).Load(path).Into(this.App.mainProfilePicture!);
            return true;
        }

        else if (upload.IsFaulted)
        { /* Todo handle upload failure */ }

        else if (upload.IsCanceled)
        { /* Todo handle upload cancelation */ }
        return false;
    }

    public async Task<bool> OnConfirmProfileUsername()
    {
        if (this.UserProfileChangeRequest.DisplayName == null)
        {
            this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
            return false;
            //username should never be null
        }

        await FirebaseAuth.Instance!.CurrentUser!.UpdateProfileAsync(this.UserProfileChangeRequest.Build());
        return true;
    }
}
