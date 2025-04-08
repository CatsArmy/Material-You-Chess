using Android.Gms.Extensions;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Chess.App.Common.Extensions;
using Chess.App.Common.Listener;
using Chess.App.Networked;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using AndroidUri = Android.Net.Uri;

namespace Chess.App;

public class ProfileFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    private Toast MissingPermissions => Toast.MakeText(this.Context, "Canceled operation, missing permissions", ToastLength.Long)!;
    private AndroidUri? Upload;
    public CircularProgressIndicator? UploadIndicator;
    public ShapeableImageView? ProfilePicture { get; set; }
    public Button? Logout { get; set; }
    public TextView? DisplayName { get; set; }
    public TextInputEditText? UsernameInput { get; set; }
    public TextInputLayout? UsernameLayout { get; set; }
    public FirebaseUserClient? UserClient;
    public ExtendedFloatingActionButton? DeleteProfilePicture;
    public ExtendedFloatingActionButton? CaptureProfilePicture;
    public ExtendedFloatingActionButton? SelectProfilePicture;
    public FirebaseUser? User;

    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />)
    /// Output is <see langword="typeof"/>(<see cref="AndroidUri" />?) </summary>
    public ActivityResultLauncher? photoPicker;

    /// <summary> Input is <see langword="typeof"/>(<see cref="AndroidUri" />) 
    /// Output is <see langword="typeof"/>(<see cref="Java.Lang.Boolean" />) </summary>
    public ActivityResultLauncher? photoTaker;
    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var file = new Java.IO.File(FileProvider.GetTemporaryRootDirectory(), "temp.image");
        this.Upload = FileProvider.GetUriForFile(file);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(), new ActivityResultCallback<AndroidUri>(this.OnPickPhoto));

        this.photoTaker = base.RegisterForActivityResult(new TakePicture(), new ActivityResultCallback<Java.Lang.Boolean>((value) => this.OnTakePhoto(value!.BooleanValue())));
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.UploadIndicator = view.FindViewById<CircularProgressIndicator>(Resource.Id.uploadIndicator);
        this.ProfilePicture = view.FindViewById<ShapeableImageView>(Resource.Id.profile_picture);
        this.Logout = view.FindViewById<Button>(Resource.Id.logout);
        this.UsernameLayout = view.FindViewById<TextInputLayout>(Resource.Id.username_layout);
        this.UsernameInput = view.FindViewById<TextInputEditText>(Resource.Id.username);
        this.DisplayName = view.FindViewById<TextView>(Resource.Id.displayname_text);
        this.DeleteProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.delete);
        this.SelectProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.library);
        this.CaptureProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.camera);
        if (FirebaseAuth.Instance?.CurrentUser is not FirebaseUser user)
        {
            Logger.Warn("FirebaseAuth.Instance?.CurrentUser is null");
            return;
        }

        this.User = user;
        this.UserClient = new FirebaseUserClient(this.User);
        Glide.With(this).Load(this.UserClient.ProfilePicture).SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
            .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
        this.DisplayName!.Text = this.UserClient.Username;
        this.UsernameInput!.Hint = this.UserClient.Username;
        this.UsernameInput!.Text = this.UserClient.Username;
        this.UsernameInput.SetImeActionLabel("Done", ImeAction.Done);
        this.UsernameInput.EditorAction += async (sender, args) =>
        {
            if (args.ActionId == ImeAction.Done)
            {
                this.DisplayName!.Text = this.UsernameInput.Text;
                var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetDisplayName(this.DisplayName.Text);
                await user.UpdateProfileAsync(changeUsernameRequest.Build());
            }
        };

        var signOut = new MaterialAlertDialogBuilder(this.Context!).SetTitle("Sign Out")?.SetIcon(Resource.Drawable.logout)
            ?.SetMessage("Are you sure you want to sign out")?.SetNeutralButton("Ok", (_, _) => FirebaseAuth.Instance?.SignOut());

        var delete = new MaterialAlertDialogBuilder(this.Context!).SetTitle("Delete")?.SetIcon(Resource.Drawable.delete)
            ?.SetMessage("Are you sure you want to delete your profile picture?\nthis action cannot be undone")
            ?.SetPositiveButton("Confirm", (_, _) => this.OnDeletePhoto())
            ?.SetNegativeButton("Cancel", (_, _) => { });

        this.Logout!.Click += (_, _) => signOut?.Show();
        this.SelectProfilePicture!.Click += (_, _) => this.PhotoPicker();
        this.SelectProfilePicture!.LongClick += (_, _) => this.SelectProfilePicture.Spin();
        this.CaptureProfilePicture!.Click += (_, _) => this.PhotoTaker();
        this.CaptureProfilePicture!.LongClick += (_, _) => this.CaptureProfilePicture.Spin();
        this.DeleteProfilePicture!.Click += (_, _) => delete?.Show();
        this.DeleteProfilePicture!.LongClick += (_, _) => this.DeleteProfilePicture.Spin();
    }

    private void PhotoTaker()
    {
        this.photoTaker?.Launch(this.Upload);
    }

    private void PhotoPicker()
    {
        this.photoPicker?.Launch(new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance).Build());
    }

    private void OnPickPhoto(AndroidUri? picked)
    {
        if (picked is null)
            return;

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var uploadTask = storageRef.PutFile(picked);
        var isSuccess = false;
        uploadTask.AddOnSuccessListener(new OnSuccess<UploadTask.TaskSnapshot>((taskSnapshot) =>
        {
            isSuccess = true;
            Logger.Debug($"uploadPhoto:onSuccess: {taskSnapshot?.Metadata?.Reference?.Path}");
            Logger.Debug($"uploadPhoto:onSuccess: {taskSnapshot?.Metadata?.Name}");
        })).AddOnFailureListener(new OnFailure((exception) =>
        {
            // Handle unsuccessful uploads
            Logger.Warn(exception.ToString());
            this.SelectProfilePicture?.OnError(this.Activity!);
        })).AddOnCompleteListener(new OnComplete(async (task) =>
        {
            if (isSuccess)
            {
                Glide.With(this).Load(storageRef).SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
                .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
                var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
                await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
                this.SelectProfilePicture?.Spin();
            }
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }));
    }

    private void OnTakePhoto(bool isTaken)
    {
        if (isTaken is false)
            return;

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var uploadTask = storageRef.PutFile(this.Upload!);
        var isSuccess = false;
        uploadTask.AddOnSuccessListener(new OnSuccess<UploadTask.TaskSnapshot>((taskSnapshot) =>
        {
            isSuccess = true;
            Logger.Debug($"uploadPhoto:onSuccess: {taskSnapshot?.Metadata?.Reference?.Path}");
            Logger.Debug($"uploadPhoto:onSuccess: {taskSnapshot?.Metadata?.Name}");
        })).AddOnFailureListener(new OnFailure((exception) =>
        {
            // Handle unsuccessful uploads
            Logger.Warn(exception.ToString());
            this.CaptureProfilePicture?.OnError(this.Activity!);
        })).AddOnCompleteListener(new OnComplete(async (task) =>
        {
            if (isSuccess)
            {
                Glide.With(this).Load(storageRef).SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
                .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);

                var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
                await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
                this.CaptureProfilePicture?.Spin();
            }
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }));
    }

    public async void OnDeletePhoto()
    {
        if (this.User?.PhotoUrl == null)
        {
            this.DeleteProfilePicture?.Spin();
            return;
        }

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var deleteTask = storageRef.DeleteAsync(); await deleteTask;
        if (deleteTask.IsCompletedSuccessfully) // bug: cant load the Drawable.outline_account_circle_24 resource
        {
            Glide.With(this).Clear(this.ProfilePicture!);
            this.DeleteProfilePicture?.Spin();
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
            var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(null);
            await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
        }

        if (deleteTask.IsFaulted) // Handle unsuccessful delete
        {
            Logger.Warn($"{deleteTask.Exception}");
            this.DeleteProfilePicture?.OnError(this.Activity!);
        }
    }
}

