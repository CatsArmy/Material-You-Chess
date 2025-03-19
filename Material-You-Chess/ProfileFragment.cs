using Android.Graphics;
using Android.Views;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;
using Google.Android.Material.TextField;
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
    public ActivityResultLauncher<PickVisualMediaRequest>? photoPicker;
    public PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;

    private PermissionsRequester? permissionsHandler;
    private FirebaseAuth? Auth;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        this.photoPicker = this.RegisterForActivityResult<PickVisualMediaRequest, AndroidUri>(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(this.SelectPhoto));

        this.PhotoTaker = this.RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.CapturePhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance);
        this.Auth = Main_Activity.Instance.Auth!;
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.permissionsHandler = new(this.Activity!);

        this.ProfilePicture = view.FindViewById<ShapeableImageView>(Resource.Id.profile_picture);
        this.ThemeToggle = view.FindViewById<MaterialSwitch>(Resource.Id.theme_switch);
        this.Logout = view.FindViewById<Button>(Resource.Id.logout);
        this.DisplayName = view.FindViewById<TextView>(Resource.Id.displayname_text);
        this.DeleteProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.delete);
        this.SelectProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.library);
        this.CaptureProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.camera);

        //if (FirebaseAuth.Instance?.CurrentUser?.DisplayName == null || FirebaseAuth.Instance?.CurrentUser?.DisplayName == string.Empty)
        //    Logger.Debug("Display name is missing???");

        //this.ThemeToggle!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        //this.UsernameInput!.Hint = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        //this.UsernameInput!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        //this.ProfilePicture!.SetImageURI(null);

        //if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
        //{
        //    //Glide.With(view).Load(FirebaseStorage.Instance.Reference
        //    //.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png")).Error(Resource.Drawable.outline_account_circle_24)
        //    //.Into(this.DialogProfilePicture!);

        //    //TODO redo the way i store profile pictures
        //}

        //this.ThemeToggle!.CheckedChange += this.ThemeChanged;
        //this.ThemeToggle!.Checked = .App.MaterialYouThemePreference;
        this.Logout!.Click += (_, _) => this.Auth?.SignOut();
        this.SelectProfilePicture!.Click += this.OpenPhotoPicker;
        this.CaptureProfilePicture!.Click += this.OpenPhotoTaker;
        //this.DeleteProfilePicture!.Click +=
    }

    public void OpenPhotoTaker(object? sender, EventArgs args)
    {
        //if (this.permissionsHandler?.Camera != Permission.Granted)
        //{
        //    this.permissionsHandler?.RequestCamaraAccess();
        //    return;
        //}

        this.PhotoTaker?.Launch(null);
        return;
    }

    //[RequiresPermission()]
    public void OpenPhotoPicker(object? sender, EventArgs args)
    {
        //if (!this.permissionsHandler!.HasMediaAccess())
        //{
        //    this.permissionsHandler.RequestMediaAccess();
        //    return;
        //}
        this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());
        return;
    }

    private void CapturePhoto(Bitmap? photo) => this.OnSelectPhoto(photo);
    private void OnSelectPhoto(Bitmap? photo) => Glide.With(this.Context!).Load(photo)
        .Error(Resource.Drawable.outline_account_circle_24).Into(this.ProfilePicture!);
    private void SelectPhoto(AndroidUri? photo)
    {
        if (photo is null)
            return;

        this.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.Activity!.ContentResolver!, photo)));
    }

}
