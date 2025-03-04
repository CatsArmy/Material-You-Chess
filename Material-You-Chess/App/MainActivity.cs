using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.Activity.Result;
using AndroidX.AppCompat.App;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Chess.App.Networked;
using Chess.Dialogs;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Java.IO;
using Microsoft.Maui.ApplicationModel;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using AndroidUri = Android.Net.Uri;

namespace Chess.App;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    public bool MaterialYouThemePreference
    {
        get; set
        {
            field = value;
            if (value == true)
            {
                base.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);
            }
            if (value == false)
            {
                base.SetTheme(Resource.Style.AppTheme_Material3_DayNight_NoActionBar);
            }
        }
    } = true;

    public CircularProgressIndicator? UserProgressIndicator;
    public ShapeableImageView? mainProfilePicture;
    public ActivityResultLauncher? PhotoTaker;
    public ActivityResultLauncher<PickVisualMediaRequest>? photoPicker;
    public PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;
    public ProfileDialog? profileDialog;
    private LogoutDialog? logoutDialog;
    private LoginDialog? loginDialog;
    private SignupDialog? signupDialog;
    private MaterialButtonToggleGroup? GameModeSelector;
    private Button? Online;
    private Button? Local;
    private Button? startGame;
    private TextView? mainUsername;
    private ExtendedFloatingActionButton? profileAction1;
    private ExtendedFloatingActionButton? profileAction2;
    private PermissionsRequester? permissionsHandler;

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

    private void CapturePhoto(Bitmap photo) => this.profileDialog?.OnSelectPhoto(photo);

    private void SelectPhoto(AndroidUri photo) => this.profileDialog?.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.ContentResolver!, photo)));

    private void StartGame(object? sender, EventArgs e)
    {
        Intent intent;
        if (this.GameModeSelector!.CheckedButtonId == this.Online!.Id)
        {
            //if (this.permissionsHandler!.HasNearbyAccess())
            //{
            intent = new Intent(this, typeof(NetworkedChessActivity))
           .PutExtra(nameof(this.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}");
            base.StartActivity(intent);
            return;
            //}
            //this.permissionsHandler!.RequestNearbyConnectionsAccess();
            //return;
        }

        intent = new Intent(this, typeof(ChessActivity))
        .PutExtra(nameof(this.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}");
        base.StartActivity(intent);
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _ = this.GetMaterialYouThemePreference(out bool MaterialYouThemePreference);
        this.MaterialYouThemePreference = MaterialYouThemePreference;

        this.photoPicker = new(base.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(this.SelectPhoto)));

        this.PhotoTaker = base.RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.CapturePhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);

        using var glide = Glide.Get(this);
        {
            glide.Registry.Append(Java.Lang.Class.FromType(typeof(StorageReference)),
            Java.Lang.Class.FromType(typeof(InputStream)), new FirebaseImageLoader.Factory());
        }

        this.GameModeSelector = this.FindViewById<MaterialButtonToggleGroup>(Resource.Id.GameModeSelector);
        this.Online = this.FindViewById<Button>(Resource.Id.btnOnline);
        this.Local = this.FindViewById<Button>(Resource.Id.btnLocal);
        this.GameModeSelector!.Check(this.Local!.Id);
        this.startGame = this.FindViewById<Button>(Resource.Id.btnStartGame);
        this.UserProgressIndicator = this.FindViewById<CircularProgressIndicator>(Resource.Id.UserProgressIndicator);
        this.mainProfilePicture = this.FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        this.mainUsername = this.FindViewById<TextView>(Resource.Id.MainUsername);
        this.profileAction1 = this.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = this.FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        this.startGame!.Click += this.StartGame;

        // Use your app or activity context to instantiate a client instance of CredentialManager.
        //var credentialManager = CredentialManager.Create(this.ApplicationContext!);

    }

    protected override void OnStart()
    {
        base.OnStart();
        this.permissionsHandler = new PermissionsRequester(this);
        this.logoutDialog = new LogoutDialog(this);
        this.loginDialog = new LoginDialog(this);
        this.signupDialog = new SignupDialog(this);
        this.profileDialog = new ProfileDialog(this);
        this.UpdateUserState();
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        // Handle permission requests results
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        try
        {
            this.permissionsHandler?.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        catch (Exception) { }
    }

    public override void OnCreateContextMenu(IContextMenu? menu, View? view, IContextMenuContextMenuInfo? menuInfo)
    {
        base.OnCreateContextMenu(menu, view, menuInfo);
        base.MenuInflater.Inflate(Resource.Menu.clear_pfp, menu);
    }

    public override bool OnContextItemSelected(IMenuItem item)
    {
        if (item.ItemId == Resource.Id.clear)
        {
            this.profileDialog?.OnClearPhoto();
        }
        return base.OnContextItemSelected(item);
    }

    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility")]
    public void UpdateUserState()
    {
        switch (FirebaseAuth.Instance.CurrentUser != null)
        {
            case true:
                this.Online!.Enabled = true;
                this.UserProgressIndicator?.Show();
                this.UserProgressIndicator?.Hide();
                this.profileAction1!.Text = "Profile";
                this.profileAction1.Click -= this.loginDialog!.Show;
                this.profileAction1.Click += this.profileDialog!.Show;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);

                this.profileAction2!.Text = "Log out";
                this.profileAction2.Click -= this.signupDialog!.Show;
                this.profileAction2.Click += this.logoutDialog!.Show;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);

                this.mainUsername!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    //var path = $"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png";
                    //Potential fix? .AsBitmap(), Downside not sure if it will always download it
                    //Glide.With(this).AsBitmap().Load(FirebaseStorage.Instance.Reference.Child(path)).Error(Resource.Drawable.outline_account_circle_24)
                    //    .Into(this.mainProfilePicture!);
                }
                break;

            case false:
                this.Online!.Enabled = false;
                this.GameModeSelector!.ClearChecked();

                this.profileAction1!.Text = "Login";
                this.profileAction1.Click -= this.OpenProfileDialog;
                this.profileAction1.Click += this.OpenLoginDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);

                this.profileAction2!.Text = "Sign up";
                this.profileAction2.Click -= this.OpenLogoutDialog;
                this.profileAction2.Click += this.OpenSignupDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_add);

                this.mainUsername!.Text = "Guest";
                Glide.With(this).Clear(this.mainProfilePicture!);
                this.mainProfilePicture!.SetImageURI(null);
                this.mainProfilePicture.RequestLayout();
                break;
        }
    }

    private void OpenLogoutDialog(object? sender, EventArgs args) => this.logoutDialog?.Dialog.Show();
    private void OpenProfileDialog(object? sender, EventArgs args) => this.profileDialog?.Dialog.Show();
    private void OpenLoginDialog(object? sender, EventArgs args) => this.loginDialog?.Dialog.Show();
    private void OpenSignupDialog(object? sender, EventArgs args) => this.signupDialog?.Dialog.Show();
}