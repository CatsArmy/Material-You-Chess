using Android.Gms.Extensions;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Chess.App.Common.Extensions;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseUI.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using AndroidUri = Android.Net.Uri;

namespace Chess.App;

public class ProfileFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    private MainActivity? activity;
    private AndroidUri? Upload;

    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />)
    /// Output is <see langword="typeof"/>(<see cref="AndroidUri" />?) </summary>
    public ActivityResultLauncher? photoPicker;

    /// <summary> Input is <see langword="typeof"/>(<see cref="AndroidUri" />) 
    /// Output is <see langword="typeof"/>(<see cref="Java.Lang.Boolean" />) </summary>
    public ActivityResultLauncher? photoTaker;

    public ActivityResultLauncher? SignInLauncher;
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

    private AndroidX.AppCompat.App.AlertDialog.Builder? Delete => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Delete")
    ?.SetIcon(Resource.Drawable.delete)
    ?.SetMessage("Are you sure you want to delete your profile picture?\nthis action cannot be undone")
    ?.SetPositiveButton("Confirm", async (_, _) => await this.OnDeletePhoto())
    ?.SetNegativeButton("Cancel", (_, _) => { });

    private AndroidX.AppCompat.App.AlertDialog.Builder? SignOut => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Sign Out")
    ?.SetIcon(Resource.Drawable.logout)
    ?.SetMessage("Are you sure you want to sign out")
    ?.SetNeutralButton("Ok", (_, _) => FirebaseAuth.Instance?.SignOut());

    /// <summary>Loads the profile picture from the cache or firebase storage</summary>
    private RequestBuilder? Thumbnail => Glide.With(this).Load(this.UserClient?.ProfilePicture)
        .SkipMemoryCache(false).SetDiskCacheStrategy(DiskCacheStrategy.All!);

    private bool ButtonsEnabled
    {
        set
        {
            this.Logout!.Enabled = value;
            this.SelectProfilePicture!.Enabled = value;
            this.CaptureProfilePicture!.Enabled = value;
            this.DeleteProfilePicture!.Enabled = value;
        }
    }

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.activity = this.Activity as MainActivity;

        var file = new Java.IO.File(FileProvider.GetTemporaryRootDirectory(), "temp.image");
        this.Upload = FileProvider.GetUriForFile(file);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(),
            new ActivityResultCallback<AndroidUri>(async (picked) => await this.OnPickPhoto(picked)));

        this.photoTaker = base.RegisterForActivityResult(new TakePicture(), new ActivityResultCallback<Java.Lang.Boolean>(async (value) => await this.OnTakePhoto(value!.BooleanValue()))); // convert the java.lang.boolean to a c# bool

        this.SignInLauncher = base.RegisterForActivityResult(contract: new FirebaseAuthUIActivityResultContract(),
            callback: new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));
    }

    /// <remarks>Called when the views are ready to be bound to this class</remarks>
    /// <summary> binds all the views and updates(<see cref="OnLoggedIn(FirebaseUser)"/>) them to show the current user info </summary>
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

        //add OnClick event listeners and disable the buttons until we confirm the user is logged in
        this.Logout!.Click += (_, _) => this.SignOut?.Show();
        this.SelectProfilePicture!.Click += (_, _) => this.PhotoPicker();
        this.CaptureProfilePicture!.Click += (_, _) => this.PhotoTaker();
        this.DeleteProfilePicture!.Click += (_, _) => this.Delete?.Show();
        this.ButtonsEnabled = false;

        if (this.activity?.Auth?.CurrentUser is not FirebaseUser user) // The user is not logged in
        {
            Logger.Verbose("FirebaseAuth.Instance?.CurrentUser is null");
            this.OpenSignInIntentActivity();
            return;
        }

        this.OnLoggedIn(user);
    }

    /// <summary> sets the contents of the views to match the user info and sets the onClickListeners for them </summary>
    /// <param name="user">the currently logged in user</param>
    private void OnLoggedIn(FirebaseUser user)
    {
        this.User = user;
        this.UserClient = new FirebaseUserClient(this.User);
        Glide.With(this).Load(this.UserClient.ProfilePicture).SkipMemoryCache(true)
            .SetDiskCacheStrategy(DiskCacheStrategy.None!)
            .Placeholder(this.ProfilePicture!.Drawable!)
            .Thumbnail(this.Thumbnail)
            .Into(this.ProfilePicture!);

        this.DisplayName!.Text = this.UserClient.Username;
        this.UsernameInput!.Hint = this.UserClient.Username;
        this.UsernameInput!.Text = this.UserClient.Username;
        this.UsernameInput.SetImeActionLabel("Done", ImeAction.Done);
        this.UsernameInput.EditorAction += async (sender, args) => await this.OnUsernameEditorAction(sender, args);

        this.ButtonsEnabled = true;
    }

    private void PhotoTaker()
    {
        this.photoTaker?.Launch(this.Upload);
    }

    private void PhotoPicker()
    {
        this.photoPicker?.Launch(new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance).Build());
    }

    private async Task OnUsernameEditorAction(object? sender, TextView.EditorActionEventArgs args)
    {
        if (args.ActionId == ImeAction.Done)
        {
            this.DisplayName!.Text = this.UsernameInput?.Text;
            var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetDisplayName(this.DisplayName.Text);
            await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
        }
    }

    /// <summary> 
    /// Uploads a selected photo to firebase and informs(animates) the user 
    /// if the operation was successful or not, while also handling any errors to prevent crashes 
    /// </summary>
    /// <param name="picked">a <see cref="AndroidUri"/> to the photo for uploading it</param>
    private async Task OnPickPhoto(AndroidUri? picked)
    {
        if (picked is null)
            return;

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var uploadTask = storageRef.PutFile(picked).AsAsync<UploadTask.TaskSnapshot>();
        var snapshot = await uploadTask;
        var fab = this.SelectProfilePicture;
        if (uploadTask.IsCompletedSuccessfully)
        {
            Logger.Debug(snapshot.Metadata!.Path);
            Glide.With(this).Load(storageRef).SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
            .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);

            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
            fab?.Spin();
        }

        if (uploadTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            fab?.OnError(this.Activity!);
        }

        if (uploadTask.IsCompleted)
        {
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }
    }

    /// <summary>
    /// Uploads a photo that we captured into the <see cref="AndroidUri"/> <see cref="Upload"/> to firebase 
    /// and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
    /// <param name="isTaken">
    /// a boolean that informs the function whether or not the user captured a photo 
    /// or if the user has canceled the operation
    /// </param>
    private async Task OnTakePhoto(bool isTaken)
    {
        if (isTaken is false)
            return;

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var uploadTask = storageRef.PutFile(this.Upload!).AsAsync<UploadTask.TaskSnapshot>();
        var snapshot = await uploadTask;
        var fab = this.CaptureProfilePicture;
        if (uploadTask.IsCompletedSuccessfully)
        {
            Logger.Debug(snapshot.Metadata!.Path);
            Glide.With(this).Load(storageRef).SetDiskCacheStrategy(DiskCacheStrategy.None!)
            .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);

            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
            fab?.Spin();
        }

        if (uploadTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            fab?.OnError(this.Activity!);
        }

        if (uploadTask.IsCompleted)
        {
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }
    }

    /// <summary>
    /// Deletes the profile picture that was uploaded to firebase and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
    public async Task OnDeletePhoto()
    {
        if (this.User?.PhotoUrl == null)
        {
            this.DeleteProfilePicture?.Spin();
            return;
        }

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var deleteTask = storageRef.DeleteAsync(); await deleteTask;
        var fab = this.DeleteProfilePicture;

        if (deleteTask.IsCompletedSuccessfully)
        {
            Glide.With(this).Clear(this.ProfilePicture!);

            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(null);
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
            fab?.Spin();
        }

        if (deleteTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            fab?.OnError(this.Activity!);
        }

        if (deleteTask.IsCompleted)
        {
            this.UploadIndicator!.Visibility = ViewStates.Invisible;
        }
    }

    /// <summary>
    /// opens the Sign Up/In activity from the NuGet Package: "FirebaseUI" 
    /// and sets the available sign-in providers to be either: sign-in via email or sign-in via a private google account
    /// </summary>
    public void OpenSignInIntentActivity()
    {
        List<AuthUI.IdpConfig> providers = [
            new AuthUI.IdpConfig.GoogleBuilder().Build(),
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
        ];

        // Create and launch sign-in intent
        var signInIntentBuilder = AuthUI.Instance.CreateSignInIntentBuilder();
        signInIntentBuilder.SetAvailableProviders(providers);
        #region Customize the FirebaseUI-Auth sign-in/up screen
        signInIntentBuilder.SetTheme(Resource.Style.Theme_Material3_DynamicColors_DayNight);
        signInIntentBuilder.SetLogo(Resource.Drawable.ic_launcher_foreground);
        signInIntentBuilder.SetCredentialManagerEnabled(true);
        //signInIntentBuilder.SetAlwaysShowSignInMethodScreen(true);
        signInIntentBuilder.EnableAnonymousUsersAutoUpgrade();
        signInIntentBuilder.SetLockOrientation(true);
        #endregion
        var signInIntent = signInIntentBuilder.Build();

        // Attempt to Sign In/Up the device
        this.SignInLauncher?.Launch(signInIntent);
    }

    /// <summary>
    /// navigates the user back to the <see cref="MainFragment"/> in the case that the sign in/up operation fails to finish
    /// due to the user canceling the operation or the operation erroring out
    /// </summary>
    private void OnSignInResult(FirebaseAuthUIAuthenticationResult? result)
    {
        var resultCode = result?.ResultCode.IntValue();
        var response = result?.IdpResponse;

        if (resultCode == ((int)Result.Ok) || response == null) // Successfully signed in
        {
            if (this.activity?.Auth?.CurrentUser is not FirebaseUser user)
            {
                Logger.Warn("FirebaseAuth.Instance?.CurrentUser is null");
                Snackbar.Make(this.activity!.FragmentContainer!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
                this.activity!.NavigationBar!.SelectedItemId = this.activity!.MainItem!.ItemId;
                return;
            }

            this.OnLoggedIn(user);
            return;
        }

        if (response.Error?.ErrorCode == ErrorCodes.NoNetwork) //Sign in failed
        {
            Snackbar.Make(this.activity!.FragmentContainer!, Resource.String.no_internet_connection, Snackbar.LengthLong);
            this.activity!.NavigationBar!.SelectedItemId = this.activity!.MainItem!.ItemId;
            return;
        }

        Snackbar.Make(this.activity!.FragmentContainer!, Resource.String.unknown_error, Snackbar.LengthLong).Show();
    }
}

