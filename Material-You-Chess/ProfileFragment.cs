using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.App;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
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
    public Bitmap? PhotoBitmap { get; set; } = null;
    public ExtendedFloatingActionButton? DeleteProfilePicture;
    public ExtendedFloatingActionButton? CaptureProfilePicture;
    public ExtendedFloatingActionButton? SelectProfilePicture;

    public ActivityResultLauncher? PhotoTaker;
    public FirebaseUser? User;
    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />) </summary>
    public ActivityResultLauncher? photoPicker;
    public PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;

    private PermissionsRequester? permissionsHandler;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(this.SelectPhoto));

        this.PhotoTaker = base.RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.CapturePhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance);
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.permissionsHandler = new(this.Activity!);

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
        var name = $"{user.DisplayName}";

        this.DisplayName!.Text = name;
        this.UsernameInput!.Hint = name;
        this.UsernameInput!.Text = name;

        //this.ThemeToggle!.CheckedChange += this.ThemeChanged;
        //this.ThemeToggle!.Checked = .App.MaterialYouThemePreference;
        this.Logout!.Click += (_, _) => FirebaseAuth.Instance?.SignOut();
        this.SelectProfilePicture!.Click += this.OpenPhotoPicker;
        this.SelectProfilePicture!.LongClick += (_, _) => this.SelectProfilePicture.Spin();
        this.CaptureProfilePicture!.Click += this.OpenPhotoTaker;
        this.CaptureProfilePicture!.LongClick += (_, _) => this.CaptureProfilePicture.Spin();
        //this.DeleteProfilePicture!.Click +=
        this.DeleteProfilePicture!.LongClick += (_, _) => this.DeleteProfilePicture.Spin();
    }

    public void OpenPhotoTaker(object? sender, EventArgs args)
    {
        if (this.permissionsHandler?.Camera != Permission.Granted)
        {
            this.permissionsHandler?.RequestCamaraAccess(() => this.PhotoTaker?.Launch(null));
            return;
        }

        this.PhotoTaker?.Launch(null);
    }

    public void OpenPhotoPicker(object? sender, EventArgs args)
    {
        if (!this.permissionsHandler!.HasMediaAccess())
        {
            this.permissionsHandler.RequestMediaAccess(() => this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build()));
            return;
        }

        this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());
    }

    private void CapturePhoto(Bitmap? photo) => this.OnSelectPhoto(photo);

    private void OnSelectPhoto(Bitmap? photo)
    {
        Glide.With(this.Context!).Load(photo).Error(Resource.Drawable.outline_account_circle_24).Into(this.ProfilePicture!);
    }

    private void SelectPhoto(AndroidUri? photo)
    {
        if (photo is null)
            return;

        this.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.Activity!.ContentResolver!, photo)));
    }
}
