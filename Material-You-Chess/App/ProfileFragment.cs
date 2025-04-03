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
        this.UserClient.LoadProfilePicture(Glide.With(this)).Into(this.ProfilePicture!);
        this.DisplayName!.Text = UserClient.Username;
        this.UsernameInput!.Hint = UserClient.Username;
        this.UsernameInput!.Text = UserClient.Username;
        this.UsernameInput.EditorAction += (sender, args) =>
        {
            if (args.ActionId == ImeAction.Done)
            {
                var changeUsernameRequest = new UserProfileChangeRequest.Builder();
                changeUsernameRequest.SetDisplayName(this.UsernameInput.Text);
            }
        };

        this.Logout!.Click += (_, _) => FirebaseAuth.Instance?.SignOut();
        this.SelectProfilePicture!.Click += (_, _) => this.PhotoPicker();
        this.SelectProfilePicture!.LongClick += (_, _) => this.SelectProfilePicture.Spin();
        this.CaptureProfilePicture!.Click += (_, _) => this.PhotoTaker();
        this.CaptureProfilePicture!.LongClick += (_, _) => this.CaptureProfilePicture.Spin();
        this.DeleteProfilePicture!.Click += (_, _) => this.OnDeletePhoto();
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
        })).AddOnCompleteListener(new OnComplete((task) =>
        {
            if (isSuccess)
            {
                Glide.With(this).Load(storageRef).SkipMemoryCache(true)
                .SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
                .Error(Resource.Drawable.outline_account_circle_24).Into(this.ProfilePicture!);
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
        })).AddOnCompleteListener(new OnComplete((task) =>
        {
            if (isSuccess)
            {
                Glide.With(this).Load(storageRef).SkipMemoryCache(true)
                .SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
                .Error(Resource.Drawable.outline_account_circle_24).Error(Resource.Drawable.outline_account_circle_24)
                .Into(this.ProfilePicture!);
                this.CaptureProfilePicture?.Spin();
            }
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }));
    }

    public async void OnDeletePhoto()
    {
        try
        {
            this.UploadIndicator!.Visibility = ViewStates.Visible;
            var storageRef = this.UserClient!.ProfilePicture;
            var deleteTask = storageRef.DeleteAsync(); await deleteTask;

            if (deleteTask.IsCompletedSuccessfully) // bug: cant load the Drawable.outline_account_circle_24 resource
            {
                Glide.With(this).Clear(this.ProfilePicture!);
                this.DeleteProfilePicture?.Spin();
                this.UploadIndicator!.Visibility = ViewStates.Invisible;
            }

            if (deleteTask.IsFaulted) // Handle unsuccessful delete
            {
                Logger.Warn($"{deleteTask.Exception}");
                this.DeleteProfilePicture?.OnError(this.Activity!);
            }
        }
        catch (Exception e) // Handle deleting a non-existent file/user
        {
            Logger.Warn($"{e}");
            this.DeleteProfilePicture?.OnError(this.Activity!);
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }
    }
}

