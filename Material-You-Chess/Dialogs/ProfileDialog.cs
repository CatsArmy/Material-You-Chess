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
    private Bitmap? PhotoBitmap = null;
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
        if (!this.WasShown)
        {
            this.App.RegisterForContextMenu(this.DialogProfilePicture!);
            this.App.RegisterForContextMenu(this.EditProfilePicture!);

            this.DialogProfilePicture!.Click +=
                this.App.OpenPhotoPicker;
            this.DialogProfilePicture.Clickable = true;
            this.EditProfilePicture!.Click +=
            this.App.OpenPhotoTaker;
            this.EditProfilePicture.Clickable = true;
            this.EditUsername!.Click += (sender, args) => this.UsernameDialog?.Dialog.Show();
            this.WasShown = true;
        }
    }

    public void ClearProfilePicture()
    {
        this.PhotoUri = null;
        this.UserProfileChangeRequest.SetPhotoUri(null);
        this.DialogProfilePicture!.SetImageURI(null);
        this.DialogProfilePicture?.RequestLayout();
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

    public void OnCapturePhoto(Bitmap photo)
    {
        this.PhotoBitmap = photo;
        this.DialogProfilePicture?.SetImageBitmap(photo);
        this.DialogProfilePicture?.RequestLayout();
    }

    public void OnCapturePhoto(Android.Net.Uri uri)
    {
        this.PhotoUri = uri;
        //Glide.With(Dialog.Context).Load(uri).Into(this.DialogProfilePicture!);
        this.DialogProfilePicture?.SetImageURI(uri);
        this.DialogProfilePicture?.RequestLayout();
    }

    private void ThemeChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        this.App.MaterialYouThemePreference = e.IsChecked;
    }

    Task<UploadTask.TaskSnapshot>? upload;

    public async void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        StorageReference path = FirebaseStorage.Instance.Reference.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png");
        if (this.PhotoUri != null || this.PhotoBitmap != null)
        {
            if (this.PhotoBitmap != null)
            {
                using (var stream = new MemoryStream())
                {
                    if (await this.PhotoBitmap.CompressAsync(Bitmap.CompressFormat.Png!, 77, stream))
                    {
                        var data = stream.ToArray();
                        upload = path.PutBytes(data).AsAsync<UploadTask.TaskSnapshot>();
                    }
                }
            }
            else if (this.PhotoUri != null)
            {
                upload = path.PutFile(this.PhotoUri!).AsAsync<UploadTask.TaskSnapshot>();
            }
            else
            {
                //temp so it would shut up
                //upload = path.PutFile(this.PhotoUri!).AsAsync<UploadTask.TaskSnapshot>();
                //Todo handle this bs
                return;
            }

            var snapshot = await upload!;
            if (upload.IsCompletedSuccessfully)
            {
                //var dir = new Java.IO.File(this.App.FilesDir, "profile");
                //var Upload = Java.IO.File.CreateTempFile("picture", ".png", dir);
                //var photoUrl = await path.GetDownloadUrlAsync();
                //var photoTask = path.GetFile(photoUrl).AsAsync<FileDownloadTask.TaskSnapshot>();
                //var result = await photoTask;
                //if (photoTask.IsCompletedSuccessfully)
                //{
                //}
                //this.UserProfileChangeRequest.SetPhotoUri(photoUrl);
                //this.App.mainProfilePicture.SetImageURI(photoUrl);
                //Glide.With(this.App!).Load(path).Into(this.App.mainProfilePicture!);
                Glide.With(this.App).Load(path).Into(this.App.mainProfilePicture!);
            }
            else if (upload.IsFaulted)
            {
                //Todo handle upload failure
            }
            else if (upload.IsCanceled)
            {
                //Todo handle upload cancelation
            }
        }
        else if (this.UserProfileChangeRequest.PhotoUri == null)
        {
            var delete = path.DeleteAsync();
            await delete;
            if (delete.IsCompletedSuccessfully)
            {
                //Todo handle delete success and inform user
            }
            else if (delete.IsFaulted)
            {
                //Todo handle delete failure
            }
            else if (delete.IsCanceled)
            {
                //Todo handle delete cancelation
            }
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
