using Android.Content.PM;
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
using AndroidUri = Android.Net.Uri;

namespace Chess;

public class ProfileFragment() : AndroidX.Fragment.App.Fragment()
{
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; } = new();
    public ShapeableImageView? ProfilePicture { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextInputEditText? UsernameInput { get; set; }
    public TextInputLayout? UsernameLayout { get; set; }
    public Bitmap? PhotoBitmap { get; set; } = null;
    public ExtendedFloatingActionButton? DeleteProfilePicture;
    public ExtendedFloatingActionButton? CaptureProfilePicture;
    public ExtendedFloatingActionButton? SelectProfilePicture;

    public ActivityResultLauncher? PhotoTaker;
    public ActivityResultLauncher<PickVisualMediaRequest>? photoPicker;
    public PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;

    private View? Root;
    private PermissionsRequester? permissionsHandler;

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.__profile_fragment__, container, false);

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.Root = view;
        this.permissionsHandler = new(this.Activity!);

        this.ProfilePicture = view.FindViewById<ShapeableImageView>(Resource.Id.profile_picture);
        this.ThemeToggle = view.FindViewById<MaterialSwitch>(Resource.Id.theme_switch);
        //this.ThemeToggle!.CheckedChange += this.ThemeChanged;
        //this.ThemeToggle!.Checked = .App.MaterialYouThemePreference;
        this.DeleteProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.delete);
        this.SelectProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.library);
        this.CaptureProfilePicture = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.camera);

        if (FirebaseAuth.Instance?.CurrentUser?.DisplayName == null || FirebaseAuth.Instance?.CurrentUser?.DisplayName == string.Empty)
            Logger.Debug("Display name is missing???");

        this.UserProfileChangeRequest.SetDisplayName(FirebaseAuth.Instance?.CurrentUser?.DisplayName);
        this.UserProfileChangeRequest.SetPhotoUri(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
        this.UsernameInput!.Hint = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        this.ProfilePicture!.SetImageURI(null);

        if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
        {
            //Glide.With(view).Load(FirebaseStorage.Instance.Reference
            //.Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png")).Error(Resource.Drawable.outline_account_circle_24)
            //.Into(this.DialogProfilePicture!);

            //TODO redo the way i store profile pictures
        }

        this.SelectProfilePicture!.Click += this.OpenPhotoPicker;
        this.CaptureProfilePicture!.Click += this.OpenPhotoTaker;
        //this.DeleteProfilePicture!.Click +=
    }

    public void OpenPhotoTaker(object? sender, EventArgs args)
    {
        if (this.permissionsHandler?.Camera == Permission.Granted)
        {
            this.PhotoTaker?.Launch(null);
            return;
        }

        this.permissionsHandler?.RequestCamaraAccess();
    }

    public void OpenPhotoPicker(object? sender, EventArgs args)
    {
        if (this.permissionsHandler!.HasMediaAccess())
        {
            this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());
            return;
        }

        this.permissionsHandler.RequestMediaAccess();
    }

    private void CapturePhoto(Bitmap? photo) => this.OnSelectPhoto(photo);
    private void OnSelectPhoto(Bitmap? photo) => Glide.With(this.Context!)
        .Load(photo).Error(Resource.Drawable.outline_account_circle_24).Into(this.ProfilePicture!);
    private void SelectPhoto(AndroidUri? photo)
    {
        if (photo is null)
        {
            this.OnSelectPhoto(null);
            return;
        }

        this.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.Activity!.ContentResolver!, photo)));
    }

}
