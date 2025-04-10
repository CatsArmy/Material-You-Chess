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
    private AndroidUri? Upload;
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
    private MainActivity? activity;

    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />)
    /// Output is <see langword="typeof"/>(<see cref="AndroidUri" />?) </summary>
    public ActivityResultLauncher? photoPicker;

    /// <summary> Input is <see langword="typeof"/>(<see cref="AndroidUri" />) 
    /// Output is <see langword="typeof"/>(<see cref="Java.Lang.Boolean" />) </summary>
    public ActivityResultLauncher? photoTaker;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.activity = this.Activity as MainActivity;

        var file = new Java.IO.File(FileProvider.GetTemporaryRootDirectory(), "temp.image");
        this.Upload = FileProvider.GetUriForFile(file);

        this.photoPicker = base.RegisterForActivityResult(new PickVisualMedia(), new ActivityResultCallback<AndroidUri>(this.OnPickPhoto));

        this.photoTaker = base.RegisterForActivityResult(new TakePicture(), new ActivityResultCallback<Java.Lang.Boolean>(async (value) => this.OnTakePhoto(value!.BooleanValue())));

        this.SignInLauncher = base.RegisterForActivityResult(contract: new FirebaseAuthUIActivityResultContract(),
            callback: new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));
    }

    /// <summary>
    /// binds all the views and updates(<see cref="OnLoggedIn(FirebaseUser)"/>) them to show the current user info 
    /// </summary>
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
        if (this.activity?.Auth?.CurrentUser is not FirebaseUser user)
        {
            Logger.Warn("FirebaseAuth.Instance?.CurrentUser is null");
            this.OpenSignInIntentActivity();
            return;
        }

        this.OnLoggedIn(user);
    }

    /// <summary>
    /// sets the contents of the views to match the user info
    /// and sets the onClickListeners for them
    /// </summary>
    /// <param name="user"></param>
    private void OnLoggedIn(FirebaseUser user)
    {
        this.User = user;
        this.UserClient = new FirebaseUserClient(this.User);
        Glide.With(this).Load(this.UserClient.ProfilePicture).SetDiskCacheStrategy(Bumptech.Glide.Load.Engine.DiskCacheStrategy.None!)
            .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
        this.DisplayName!.Text = this.UserClient.Username;
        this.UsernameInput!.Hint = this.UserClient.Username;
        this.UsernameInput!.Text = this.UserClient.Username;
        this.UsernameInput.SetImeActionLabel("Done", ImeAction.Done);
        this.UsernameInput.EditorAction += this.OnUsernameEditorAction;

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

    private async void OnUsernameEditorAction(object? sender, TextView.EditorActionEventArgs args)
    {
        if (args.ActionId == ImeAction.Done)
        {
            this.DisplayName!.Text = this.UsernameInput?.Text;
            var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetDisplayName(this.DisplayName.Text);
            await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
        }
    }

    /// <summary>
    /// Uploads a selected photo to firebase and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
    /// <param name="picked">a <see cref="AndroidUri"/> to the photo</param>
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
        })).AddOnFailureListener(new OnFailure((exception) => // Handle unsuccessful uploads
        {
            Logger.Warn(exception.ToString());
            this.SelectProfilePicture?.OnError(this.Activity!);
        })).AddOnCompleteListener(new OnComplete(async (task) => // Handle upload success
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

    /// <summary>
    /// Uploads a photo that we captured into the <see cref="AndroidUri"/> <see cref="Upload"/> to firebase 
    /// and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
    /// <param name="isTaken">
    /// a boolean that informs the function whether or not the user captured a photo 
    /// or if the user has canceled the operation
    /// </param>
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

    /// <summary>
    /// Deletes the profile picture that was uploaded to firebase and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
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

    /// <summary>
    /// opens the Sign Up/In activity from the NuGet Package: "FirebaseUI" 
    /// and sets the available signin providers to be either: signin via email or signin via a private google account
    /// </summary>
    public void OpenSignInIntentActivity()
    {
        List<AuthUI.IdpConfig> providers = [
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
            new AuthUI.IdpConfig.GoogleBuilder().Build()
        ];

        // Create and launch sign-in intent
        var signInIntentBuilder = AuthUI.Instance.CreateSignInIntentBuilder();
        signInIntentBuilder.SetAvailableProviders(providers);
        #region Customize the FirebaseUI-Auth sign-in/up screen
        signInIntentBuilder.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);
        signInIntentBuilder.SetLogo(Resource.Drawable.ic_launcher_foreground);
        signInIntentBuilder.SetCredentialManagerEnabled(true);
        signInIntentBuilder.SetAlwaysShowSignInMethodScreen(true);
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

