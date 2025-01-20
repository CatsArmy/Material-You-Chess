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
using Chess.Dialogs;
using Chess.FirebaseSecrets;
using Firebase.Auth;
using Firebase.Storage;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Microsoft.Maui.ApplicationModel;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

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

    public ActivityResultLauncher? PhotoTaker;
    private ActivityResultLauncher<PickVisualMediaRequest>? photoPicker;
    private PickVisualMediaRequest.Builder? pickVisualMediaRequestBuilder;

    public ShapeableImageView? mainProfilePicture;
    private Button? startGame;
    private TextView? mainUsername;
    private CircularProgressIndicator? UserProgressIndicator;
    private ExtendedFloatingActionButton? profileAction1;
    private ExtendedFloatingActionButton? profileAction2;
    private ProfileDialog? profileDialog;
    private LogoutDialog? logoutDialog;
    private LoginDialog? loginDialog;
    private SignupDialog? signupDialog;

    public void StartProgressIndicator() => this.UserProgressIndicator?.Show();
    public void StopProgressIndicator() => this.UserProgressIndicator?.Hide();
    public void OpenPhotoTaker(object? sender, EventArgs args) => PhotoTaker?.Launch(null);
    public void OpenPhotoPicker(object? sender, EventArgs args) => this.photoPicker?.Launch(this.pickVisualMediaRequestBuilder?.Build());

    private void CapturePhoto(Bitmap photo) => this.profileDialog?.OnSelectPhoto(photo);
    private void SelectPhoto(Android.Net.Uri photo) => this.profileDialog?.OnSelectPhoto(ImageDecoder.DecodeBitmap(ImageDecoder.CreateSource(base.ContentResolver!, photo)));
    private void StartGame(object? sender, EventArgs e)
    {
        Intent intent = new Intent(this, typeof(ChessActivity))
        .PutExtra(nameof(this.MaterialYouThemePreference), $"{this.MaterialYouThemePreference}");
        base.StartActivity(intent);
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        _ = new Secrets();
        _ = this.GetMaterialYouThemePreference(out bool MaterialYouThemePreference);
        this.MaterialYouThemePreference = MaterialYouThemePreference;

        this.photoPicker = new(this.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<Android.Net.Uri>(this.SelectPhoto)));

        this.PhotoTaker = RegisterForActivityResult(new TakePicturePreview(),
            new ActivityResultCallback<Bitmap>(this.CapturePhoto));

        this.pickVisualMediaRequestBuilder = new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        // Set our view from layout resource
        base.SetContentView(Resource.Layout.main_activity);

        //base.StartActivity(new Intent(this, typeof(MainActivity2)));
        //return;

        // Permission request logic
        _ = new PermissionsRequester(this);

        using (var glide = Glide.Get(this))
        {
            MyAppGlideModule.Register(glide);
        };

        //FirebaseAuth.Instance.SignOut();
        //Run our logic
        this.startGame = FindViewById<Button>(Resource.Id.btnStartGame);
        this.UserProgressIndicator = FindViewById<CircularProgressIndicator>(Resource.Id.UserProgressIndicator);
        this.mainProfilePicture = FindViewById<ShapeableImageView>(Resource.Id.MainProfileImageView);
        this.mainUsername = FindViewById<TextView>(Resource.Id.MainUsername);
        this.profileAction1 = FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction1);
        this.profileAction2 = FindViewById<ExtendedFloatingActionButton>(Resource.Id.profileAction2);
        this.logoutDialog = new LogoutDialog(this);
        this.loginDialog = new LoginDialog(this);
        this.signupDialog = new SignupDialog(this);
        this.profileDialog = new ProfileDialog(this);
        this.startGame!.Click += StartGame;
        this.UpdateUserState();
    }

    public override void OnCreateContextMenu(IContextMenu? menu, View? v, IContextMenuContextMenuInfo? menuInfo)
    {
        base.OnCreateContextMenu(menu, v, menuInfo);
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

    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "<Pending>")]
    public void UpdateUserState()
    {
        //var title = base.FindViewById<TextView>(Resource.Id.mTitlePart);

        //TypedValue typedValue;
        //typedValue = new TypedValue();
        //base.Theme!.ResolveAttribute(Resource.Attribute.colorTertiary, typedValue, true);
        //int color = ContextCompat.GetColor(this, typedValue.ResourceId);

        //var Material = Java.Lang.Integer.ToHexString(ContextCompat.GetColor(this, typedValue.ResourceId));
        //typedValue = new TypedValue();
        //base.Theme!.ResolveAttribute(Resource.Attribute.colorPrimary, typedValue, true);
        //var You = Java.Lang.Integer.ToHexString(ContextCompat.GetColor(this, typedValue.ResourceId));
        //typedValue = new TypedValue();
        //base.Theme!.ResolveAttribute(Resource.Attribute.colorSecondary, typedValue, true);
        //var Chess = Java.Lang.Integer.ToHexString(ContextCompat.GetColor(this, typedValue.ResourceId));
        //var text = Html.FromHtml($"<font color=#{Material}>Material </font><font color=#{You}>You </font><font color=#{Chess}>Chess</font>");
        ////var text = Html.FromHtml($"<font color=\"@color/Material_Title\">Material </font><font color=\"@color/You_Title\">\"You </font><font color=\"@color/Chess_Title\">Chess</font>");
        //title?.SetText(text, TextView.BufferType.Spannable);
        switch (FirebaseAuth.Instance.CurrentUser != null)
        {
            case true:
                this.profileAction1!.Text = "Profile";
                this.profileAction1.Click -= OpenLoginDialog;
                this.profileAction1.Click += OpenProfileDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_manage_accounts);

                this.profileAction2!.Text = "Log out";
                this.profileAction2.Click -= OpenSignupDialog;
                this.profileAction2.Click += OpenLogoutDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_remove);

                this.mainUsername!.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
                if (FirebaseAuth.Instance?.CurrentUser?.PhotoUrl is not null)
                {
                    //Java.Lang.IllegalArgumentException: 'Unhandled class: class java.io.File, try .as*(Class).transcode(ResourceTranscoder)'
                    //var load = 
                    Glide.With(this).DownloadOnly().Load(FirebaseStorage.Instance.Reference
                    .Child($"{FirebaseAuth.Instance!.CurrentUser!.Uid}/ProfilePicture.png"))//.Error(Resource.Drawable.outline_account_circle_24)
                    //;
                    .Into(mainProfilePicture!);
                    //new Thread((object? requestBuilder) =>
                    //{
                    //    (requestBuilder as RequestBuilder)?.Into(this.mainProfilePicture!);
                    //});//.Start(load);
                }
                break;

            case false:
                this.profileAction1!.Text = "Login";
                this.profileAction1.Click -= OpenProfileDialog;
                this.profileAction1.Click += OpenLoginDialog;
                this.profileAction1.SetIconResource(Resource.Drawable.outline_person);

                this.profileAction2!.Text = "Sign up";
                this.profileAction2.Click -= OpenLogoutDialog;
                this.profileAction2.Click += OpenSignupDialog;
                this.profileAction2.SetIconResource(Resource.Drawable.outline_person_add);

                this.mainUsername!.Text = "Guest";
                var unload = Glide.With(this);
                new Thread((requestBuilder) => { (requestBuilder as RequestManager)?.Clear(this.mainProfilePicture!); }).Start(unload);
                this.mainProfilePicture!.SetImageURI(null);
                this.mainProfilePicture.RequestLayout();
                break;
        }
    }

    private void OpenLogoutDialog(object? sender, EventArgs args) => this.logoutDialog?.Dialog.Show();
    private void OpenProfileDialog(object? sender, EventArgs args) => this.profileDialog?.Dialog.Show();
    private void OpenLoginDialog(object? sender, EventArgs args) => this.loginDialog?.Dialog.Show();
    private void OpenSignupDialog(object? sender, EventArgs args) => this.signupDialog?.Dialog.Show();

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}