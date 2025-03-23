using Android.Graphics;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Chess.App.Common.Extensions;
using Chess.App.Common.Listener;
using Chess.App.Common.Permissions;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using AndroidUri = Android.Net.Uri;

namespace Chess.App;

public class ProfileFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    public ShapeableImageView? ProfilePicture { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public Button? Logout { get; set; }
    public TextView? DisplayName { get; set; }
    public TextInputEditText? UsernameInput { get; set; }
    public TextInputLayout? UsernameLayout { get; set; }

    public ExtendedFloatingActionButton? DeleteProfilePicture;
    public ExtendedFloatingActionButton? CaptureProfilePicture;
    public ExtendedFloatingActionButton? SelectProfilePicture;

    public ActivityResultLauncher? PhotoTaker;
    public FirebaseUser? User;

    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />) </summary>
    public ActivityResultLauncher? photoPicker;
    private CameraAccess? CameraAccessPermissionManager;
    private MediaAccess? MediaAccessPermissionManager;
    private Toast MissingPermissions => Toast.MakeText(this.Context, "Canceled operation, missing permissions", ToastLength.Long)!;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(this.OnSelectPhoto));

        this.PhotoTaker = base.RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.OnCapturePhoto));

        this.CameraAccessPermissionManager = this.RegisterCameraPermissionManager(this.PhotoCapturer);
        this.MediaAccessPermissionManager = this.RegisterMediaPermissionsManager(this.PhotoPicker);
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.ProfilePicture = view.FindViewById<ShapeableImageView>(Resource.Id.profile_picture);
        this.ThemeToggle = view.FindViewById<MaterialSwitch>(Resource.Id.theme_switch);
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
        var User = new UserClient(this.User.Uid, this.User.DisplayName);
        User.LoadProfilePicture(Glide.With(this)).Into(this.ProfilePicture!);
        this.DisplayName!.Text = User.Username;
        this.UsernameInput!.Hint = User.Username;
        this.UsernameInput!.Text = User.Username;
        this.UsernameInput.EditorAction += (a, args) =>
        {
            if (args.ActionId == ImeAction.Done)
            {
                var changeUsernameRequest = new UserProfileChangeRequest.Builder();
                changeUsernameRequest.SetDisplayName(this.UsernameInput.Text);
            }
        };

        this.ThemeToggle!.Checked = this.Activity!.MaterialYouThemePreference();
        this.ThemeToggle!.CheckedChange += this.OnThemeChanged;
        this.Logout!.Click += (_, _) => FirebaseAuth.Instance?.SignOut();
        this.SelectProfilePicture!.Click += (_, _) => this.MediaAccessPermissionManager!.RequestAccess();
        this.SelectProfilePicture!.LongClick += (_, _) => this.SelectProfilePicture.Spin();
        this.CaptureProfilePicture!.Click += (_, _) => this.CameraAccessPermissionManager!.RequestAccess();
        this.CaptureProfilePicture!.LongClick += (_, _) => this.CaptureProfilePicture.Spin();
        //this.DeleteProfilePicture!.Click +=
        this.DeleteProfilePicture!.LongClick += (_, _) => this.DeleteProfilePicture.Spin();
    }

    private void OnThemeChanged(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        this.Activity!.MaterialYouThemePreference(e.IsChecked);
        int themeResId = e.IsChecked switch
        {
            true => Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar,
            false => Resource.Style.AppTheme_Material3_DayNight_NoActionBar,
        };
        this.Activity?.ApplicationContext?.SetTheme(themeResId);
        this.Activity?.BaseContext?.SetTheme(themeResId);
        this.Activity?.SetTheme(themeResId);

        this.Activity?.ApplicationContext?.Theme?.ApplyStyle(themeResId, true);
        this.Activity?.BaseContext?.Theme?.ApplyStyle(themeResId, true);
        this.Activity?.Theme?.ApplyStyle(themeResId, true);

        this.Activity?.ApplicationContext?.Theme?.Rebase();
        this.Activity?.BaseContext?.Theme?.Rebase();
        this.Activity?.Theme?.Rebase();

    }

    private void PhotoCapturer(bool isGranted)
    {
        if (!isGranted)
        {
            this.MissingPermissions.Show();
            return;
        }

        this.PhotoTaker?.Launch(null);
    }

    private void PhotoPicker(bool isGranted)
    {
        //if (!isGranted)
        //{
        //    this.MissingPermissions.Show();
        //    return;
        //}

        this.photoPicker?.Launch(new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance).Build());
    }

    private void OnCapturePhoto(Bitmap? photo)
    {
        var stream = new MemoryStream();
        photo!.Compress(Bitmap.CompressFormat.Png!, 100, stream);
        byte[] data = stream.ToArray();
        var storageRef = FirebaseStorage.Instance.Reference.Child($"{this.User!.Uid}");
        var uploadTask = storageRef.PutBytes(data);
        uploadTask.AddOnSuccessListener(new OnSuccess((taskSnapshot) =>
        {
            var glide = Glide.Get(this.Activity!);
            glide.ClearMemory();
            glide.ClearDiskCache();

            Glide.With(this).DownloadOnly()
            .Load(storageRef)
            .Error(Resource.Drawable.outline_account_circle_24)
            .Into(this.ProfilePicture!);

            this.CaptureProfilePicture?.Spin();
        }));
        uploadTask.AddOnFailureListener(new OnFailure((exception) =>
        {
            // Handle unsuccessful uploads
            Logger.Warn(exception.ToString());
            this.CaptureProfilePicture?.OnError(this.Activity!);
        }));
    }

    private void OnSelectPhoto(AndroidUri? photo)
    {
        if (photo is null)
            return;

        var storageRef = FirebaseStorage.Instance.Reference.Child($"{this.User!.Uid}");
        var uploadTask = storageRef.PutFile(photo);
        uploadTask.AddOnSuccessListener(new OnSuccess((taskSnapshot) =>
        {
            var glide = Glide.Get(this.Activity!);
            glide.ClearMemory();
            glide.ClearDiskCache();

            Glide.With(this).DownloadOnly()
            .Load(storageRef)
            .Error(Resource.Drawable.outline_account_circle_24)
            .Into(this.ProfilePicture!);

            this.SelectProfilePicture?.Spin();
        }));
        uploadTask.AddOnFailureListener(new OnFailure((exception) =>
        {
            // Handle unsuccessful uploads
            Logger.Warn(exception.ToString());
            this.SelectProfilePicture?.OnError(this.Activity!);
        }));
    }

    public void OnDeletePhoto()
    {
        var storageRef = FirebaseStorage.Instance.Reference.Child($"{this.User!.Uid}");
        var uploadTask = storageRef.Delete();

        uploadTask.AddOnSuccessListener(new OnSuccess((taskSnapshot) =>
        {
            var glide = Glide.Get(this.Activity!);
            glide.ClearMemory();
            glide.ClearDiskCache();

            Glide.With(this)
            .Load(Resource.Drawable.outline_account_circle_24)
            .Into(this.ProfilePicture!);

            this.DeleteProfilePicture?.Spin();
        }));
        uploadTask.AddOnFailureListener(new OnFailure((exception) =>
        {
            // Handle unsuccessful uploads
            Logger.Warn(exception.ToString());
            this.DeleteProfilePicture?.OnError(this.Activity!);
        }));
    }
}
