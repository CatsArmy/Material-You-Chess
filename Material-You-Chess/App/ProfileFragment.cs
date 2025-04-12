using Android.Gms.Extensions;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
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

public class ProfileFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.profile_fragment)
{
    private MainActivity? MainActivity;
    private AndroidUri? Upload;

    /// <summary> Input is <see langword="typeof"/>(<see cref="PickVisualMediaRequest" />)
    /// Output is <see langword="typeof"/>(<see cref="AndroidUri" />?) </summary>
    public ActivityResultLauncher? photoPicker;

    /// <summary> Input is <see langword="typeof"/>(<see cref="AndroidUri" />) 
    /// Output is <see langword="typeof"/>(<see cref="Java.Lang.Boolean" />) </summary>
    public ActivityResultLauncher? photoTaker;

    public ActivityResultLauncher? SignInLauncher;
    public CircularProgressIndicator? UploadIndicator;

    public Button? Logout { get; set; }
    public TextView? DisplayName { get; set; }
    public TextInputEditText? UsernameInput { get; set; }
    public TextInputLayout? UsernameLayout { get; set; }
    public ShapeableImageView? ProfilePicture { get; set; }

    public ExtendedFloatingActionButton? DeleteProfilePicture;
    public ExtendedFloatingActionButton? CaptureProfilePicture;
    public ExtendedFloatingActionButton? SelectProfilePicture;
    public FirebaseUserClient? UserClient;
    public FirebaseUser? User;

    /// <summary>Loads the profile picture from the cache or firebase storage</summary>
    private RequestBuilder? Thumbnail => Glide.With(this).Load(this.UserClient?.ProfilePicture)
        .SkipMemoryCache(false).SetDiskCacheStrategy(DiskCacheStrategy.All!);

    private AndroidX.AppCompat.App.AlertDialog.Builder? Delete => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Delete")
    ?.SetIcon(Resource.Drawable.delete)
    ?.SetMessage($"Are you sure you want to delete your profile picture?{Environment.NewLine}this action cannot be undone")
    ?.SetPositiveButton("Confirm", this.DeletePhoto)
    ?.SetNegativeButton("Cancel", this.NoOperation);

    private AndroidX.AppCompat.App.AlertDialog.Builder? SignOut => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Sign Out")
    ?.SetIcon(Resource.Drawable.logout)
    ?.SetMessage("Are you sure you want to sign out")
    ?.SetPositiveButton("Confirm", this.OnSignOut)
    ?.SetNegativeButton("Cancel", this.NoOperation);

    /// <summary> NoOp function for readability </summary>
    private void NoOperation(object? sender, EventArgs args) { }

    private async void OnSignOut(object? sender, EventArgs args)
    {
        try
        {
            var task = AuthUI.Instance.SignOut(this.MainActivity!); await task;
            if (task.IsComplete)
            {
                this.SetButtonsEnabled(false);
            }
        }
        catch (Exception) { }
    }


    private void SetButtonsEnabled(bool value)
    {
        this.Logout!.Enabled = value;
        this.SelectProfilePicture!.Enabled = value;
        this.CaptureProfilePicture!.Enabled = value;
        this.DeleteProfilePicture!.Enabled = value;
    }

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.MainActivity = this.Activity as MainActivity;

        var file = new Java.IO.File(FileProvider.GetTemporaryRootDirectory(), "temp.image");
        this.Upload = FileProvider.GetUriForFile(file);

        this.photoPicker = base.RegisterForActivityResult(contract: new PickVisualMedia(),
            callback: new ActivityResultCallback<AndroidUri>(this.PickPhoto));

        this.photoTaker = base.RegisterForActivityResult(contract: new TakePicture(),
            callback: new ActivityResultCallback<Java.Lang.Boolean>(this.TakePhoto));

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
        this.SetButtonsEnabled(false);

        if (this.MainActivity?.Auth?.CurrentUser is not FirebaseUser user) // The user is not logged in
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

        this.SetButtonsEnabled(true);
    }

    /// <summary> Wrapper method for Opening the Camera without special permission using the TakePicture ActivityResultContract</summary>
    private void PhotoTaker() => this.photoTaker?.Launch(this.Upload);

    /// <summary> Wrapper method for Opening the native PhotoPicker bottom sheet </summary>
    private void PhotoPicker() => this.photoPicker?.Launch(new PickVisualMediaRequest.Builder().SetMediaType(PickVisualMedia.ImageOnly.Instance).Build());

    private async Task OnUsernameEditorAction(object? sender, TextView.EditorActionEventArgs args)
    {
        if (args.ActionId == ImeAction.Done)
        {
            this.DisplayName!.Text = this.UsernameInput?.Text;
            var changeUsernameRequest = new UserProfileChangeRequest.Builder().SetDisplayName(this.DisplayName.Text);
            await this.User?.UpdateProfileAsync(changeUsernameRequest.Build())!;
        }
    }

    /// <summary> Wrapper method for <see cref="OnTakePhoto(bool)"/> </summary>
    public async void PickPhoto(AndroidUri? picked)
    {
        try
        {
            await this.OnPickPhoto(picked);
        }
        catch (Exception) { }
    }

    /// <summary> 
    /// Uploads a selected photo to firebase and informs(animates) the user 
    /// if the operation was successful or not, while also handling any errors to prevent crashes 
    /// </summary>
    /// <param name="picked">a <see cref="AndroidUri"/> to the photo for uploading it</param>
    private async Task OnPickPhoto(AndroidUri? picked)
    {
        if (picked is null) return;

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var uploadTask = storageRef.PutFile(picked).AsAsync<UploadTask.TaskSnapshot>();
        var snapshot = await uploadTask;
        var fab = this.SelectProfilePicture;
        if (uploadTask.IsCompletedSuccessfully)
        {
            Logger.Debug(snapshot.Metadata!.Path);
            try
            {
                Glide.With(this).Load(storageRef).SetDiskCacheStrategy(DiskCacheStrategy.None!)
                .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
                fab?.Spin();
            }
            catch (Exception) { }

            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
        }

        if (uploadTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            try
            {
                fab?.OnError(this.Activity!);
            }
            catch (Exception) { }
        }

        if (uploadTask.IsCompleted)
        {
            try
            {
                this.UploadIndicator!.Visibility = ViewStates.Invisible;
            }
            catch (Exception) { }
        }
    }

    /// <summary> Wrapper method for <see cref="OnTakePhoto(bool)"/> </summary>
    public async void TakePhoto(Java.Lang.Boolean? value)
    {
        try
        {
            await this.OnTakePhoto(value!.BooleanValue());
        }
        catch (Exception) { }
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
            try
            {
                Glide.With(this).Load(storageRef).SetDiskCacheStrategy(DiskCacheStrategy.None!)
                .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
                fab?.Spin();
            }
            catch (Exception) { }
            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(await storageRef.GetDownloadUrlAsync());
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
        }

        if (uploadTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            try
            {
                fab?.OnError(this.Activity!);
            }
            catch (Exception) { }
        }

        if (uploadTask.IsCompleted)
        {
            try
            {
                this.UploadIndicator!.Visibility = ViewStates.Invisible;
            }
            catch (Exception) { }
        }
    }

    /// <summary> Wrapper method for <see cref="OnDeletePhoto()"/> </summary>
    public async void DeletePhoto(object? sender, EventArgs args)
    {
        try
        {
            await this.OnDeletePhoto();
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Deletes the profile picture that was uploaded to firebase and informs(animates) the user if the operation was successful or not
    /// while also handling any errors to prevent crashes
    /// </summary>
    public async Task OnDeletePhoto()
    {
        var fab = this.DeleteProfilePicture;
        if (this.User?.PhotoUrl == null)
        {
            fab?.Spin();
            return;
        }

        this.UploadIndicator!.Visibility = ViewStates.Visible;
        var storageRef = this.UserClient!.ProfilePicture;
        var deleteTask = storageRef.DeleteAsync(); await deleteTask;

        if (deleteTask.IsCompletedSuccessfully)
        {
            try
            {
                Glide.With(this).Clear(this.ProfilePicture!);
                fab?.Spin();
            }
            catch (Exception) { }

            var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(null);
            await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
        }

        if (deleteTask.Exception is Exception exception)
        {
            Logger.Warn($"{exception}");
            try
            {
                fab?.OnError(this.Activity!);
            }
            catch (Exception) { }
        }

        if (deleteTask.IsCompleted)
        {
            try
            {
                this.UploadIndicator!.Visibility = ViewStates.Invisible;
            }
            catch (Exception) { }
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
        signInIntentBuilder.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);
        signInIntentBuilder.SetLogo(Resource.Drawable.ic_launcher_foreground);
        signInIntentBuilder.SetCredentialManagerEnabled(true);
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
    private async void OnSignInResult(FirebaseAuthUIAuthenticationResult? result)
    {
        var resultCode = result?.ResultCode.IntValue();
        var response = result?.IdpResponse;

        if (resultCode == ((int)Result.Ok) || response == null) // Successfully signed in
        {
            if (this.MainActivity?.Auth?.CurrentUser is not FirebaseUser user)
            {
                Logger.Warn("FirebaseAuth.Instance?.CurrentUser is null");
                Snackbar.Make(this.MainActivity!.FragmentContainer!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
                this.MainActivity!.NavigationBar!.SelectedItemId = this.MainActivity!.MainItem!.ItemId;
                return;
            }

#pragma warning disable XAOBS001 // Type or member is obsolete
            if (response?.User?.PhotoUri is not null && response.IsNewUser)
            {
                var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(null);
                await this.User?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
            }
#pragma warning restore XAOBS001 // Type or member is obsolete
            this.OnLoggedIn(user);
            return;
        }

        if (response.Error?.ErrorCode == ErrorCodes.NoNetwork) //Sign in failed
        {
            Snackbar.Make(this.MainActivity!.FragmentContainer!, Resource.String.no_internet_connection, Snackbar.LengthLong);
            this.MainActivity!.NavigationBar!.SelectedItemId = this.MainActivity!.MainItem!.ItemId;
            return;
        }

        Snackbar.Make(this.MainActivity!.FragmentContainer!, Resource.String.unknown_error, Snackbar.LengthLong).Show();
    }
}

