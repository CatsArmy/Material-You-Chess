using Android.Gms.Tasks;
using Android.Graphics;
using Android.Views;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Chess.App.Networked;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
using Java.IO;
using Java.Lang;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using AndroidUri = Android.Net.Uri;

namespace Chess;

public class ProfileFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; } = new();
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
    public PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;
    private CameraAccess? CameraAccessPermissionManager;
    private MediaAccess? MediaAccessPermissionManager;
    private Toast MissingPermissions => Toast.MakeText(this.Context, "Canceled operation, missing permissions", ToastLength.Long)!;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(this.SelectPhoto));

        this.PhotoTaker = base.RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.CapturePhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance);
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
            Logger.Warn("FirebaseAuth.Instance?.CurrentUser is not FirebaseUser user");
            return;
        }

        this.User = user;
        var User = new UserClient(this.User.Uid, this.User.DisplayName);
        User.LoadProfilePicture(Glide.With(this)).Into(this.ProfilePicture!);
        this.DisplayName!.Text = User.Username;
        this.UsernameInput!.Hint = User.Username;
        this.UsernameInput!.Text = User.Username;
        this.ThemeToggle!.CheckedChange += this.OnThemeChanged;
        this.ThemeToggle!.Checked = this.Activity!.MaterialYouThemePreference();
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
        this.Activity!.SetTheme(themeResId);
        this.Activity!.Recreate();
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
        if (!isGranted)
        {
            this.MissingPermissions.Show();
            return;
        }

        this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());
    }

    private void CapturePhoto(Bitmap? photo) => this.OnSelectPhoto(photo);

    private void OnSelectPhoto(Bitmap? photo)
    {
        Glide.With(this.Context!).Load(photo).Error(Resource.Drawable.outline_account_circle_24).Into(this.ProfilePicture!);
        var stream = new MemoryStream();

        photo!.Compress(Bitmap.CompressFormat.Png!, 100, stream);
        byte[] data = stream.ToArray();
        var storageRef = FirebaseStorage.Instance.Reference.Child($"{this.User!.Uid}.png");
        var uploadTask = storageRef.PutBytes(data);
        uploadTask.AddOnSuccessListener(new OnSuccessListener((taskSnapshot) =>
        {
            // taskSnapshot.getMetadata() contains file metadata such as size, content-type, etc.
            // ...
        }));
        uploadTask.AddOnFailureListener(new OnFailureListener((exception) =>
        {
            // Handle unsuccessful uploads
        }));

    }

    private void SelectPhoto(AndroidUri? photo)
    {
        if (photo is null)
            return;

        this.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.Activity!.ContentResolver!, photo)));
    }
}
