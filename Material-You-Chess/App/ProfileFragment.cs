using Android.Gms.Extensions;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Firebase.Auth;
using FirebaseUI.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Dialog;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using Material.You.Chess.App.Common;
using Material.You.Chess.App.Common.ActivityResult;
using Material.You.Chess.App.Common.Extensions;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Material.You.Chess.App;

public class ProfileFragment() : Fragment(Resource.Layout.profile_fragment)
{
    #region UI views that will be bound
    private Button? SignOut;
    private TextView? DisplayName;
    private TextInputLayout? UsernameLayout;
    private TextInputEditText? UsernameInput;
    private ShapeableImageView? ProfilePicture;
    private CircularProgressIndicator? UploadIndicator;
    private ExtendedFloatingActionButton? DeletePfP;
    private ExtendedFloatingActionButton? CapturePfP;
    private ExtendedFloatingActionButton? SelectPfP;
    #endregion
    /// <summary> Used to access and store(as a ref) to the "AndroidX.Fragment.App.FragmentActivity this.Activity" 
    /// as a MainActivity? MainActivity that is used for getting the FirebaseAuth instance/the current user </summary>
    private MainActivity? MainActivity;
    /// <summary> The current user must be stored because get the current user from: "FirebaseAuth.Instance.CurrentUser",
    /// more than once has a tendency to crash the app for no obvious reason </summary>
    private FirebaseUser? FirebaseUser;
    /// <summary> The user client used to receive the path to the profile picture of a user </summary>
    private Common.User? User;
    /// <summary> Input is typeof(SignInIntent) Output is typeof(FirebaseAuthUIAuthenticationResult) </summary>
    private ActivityResultLauncher? SignInLauncher;
    /// <summary> Input is typeof(PickVisualMediaRequest) Output is typeof(Android.Net.Uri(aka AndroidUri)?) </summary>
    private ActivityResultLauncher? PhotoPicker;
    /// <summary> Input is typeof(Android.Net.Uri(aka AndroidUri)) Output is typeof(Java.Lang.Boolean) </summary>
    private ActivityResultLauncher? PhotoTaker;
    private AndroidUri? TemporarilyUploadToFile;
    /// <summary> Loads the profile picture from the cache or firebase storage </summary>
    private RequestBuilder? Thumbnail => Glide.With(this).Load(this.User?.ProfilePicture).SkipMemoryCache(false).SetDiskCacheStrategy(DiskCacheStrategy.All!);
    private UserProfileChangeRequest.Builder ChangeUsernameRequest => new UserProfileChangeRequest.Builder().SetDisplayName(this.DisplayName!.Text);
    /// <returns> a delete confirmation dialog </returns>
    private AlertDialog.Builder? DeleteConfirmation
        => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Delete")?.SetIcon(Resource.Drawable.delete)
        ?.SetMessage($"Are you sure you want to delete your profile picture?{Environment.NewLine}this action cannot be undone")
        ?.SetPositiveButton("Confirm", async (sender, args) => await this.OnDeletePhoto())
        ?.SetNegativeButton("Cancel", (_, _) => { });
    /// <returns> a sign-out confirmation dialog </returns>
    private AlertDialog.Builder? SignOutConfirmation
        => new MaterialAlertDialogBuilder(this.Context!)?.SetTitle("Sign Out")?.SetIcon(Resource.Drawable.logout)
        ?.SetMessage("Are you sure you want to sign out")
        ?.SetPositiveButton("Confirm", async (sender, args)
            => await this.OnUpdateUI(AuthUI.Instance.SignOut(this.MainActivity!).AsAsync(), this.SignOut))
        ?.SetNegativeButton("Cancel", (_, _) => { });
    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.MainActivity = this.Activity as MainActivity;
        var file = new Java.IO.File(FileProvider.GetTemporaryRootDirectory(), "temp.image");
        this.TemporarilyUploadToFile = FileProvider.GetUriForFile(file);
        this.PhotoPicker = base.RegisterForActivityResult(contract: new PickVisualMedia(),
            callback: new ActivityResultCallback<AndroidUri>(this.PickPhoto));
        this.PhotoTaker = base.RegisterForActivityResult(contract: new TakePicture(),
            callback: new ActivityResultCallback<Java.Lang.Boolean>(this.TakePhoto));
        this.SignInLauncher = base.RegisterForActivityResult(contract: new FirebaseAuthUIActivityResultContract(),
            callback: new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));
    }

    /// <remarks> Called when the views are ready to be bound </remarks>
    /// <summary> binds all the views and refreshes the ui (via OnLoggedIn(FirebaseUser)) them to show the current user info </summary>
    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.UploadIndicator = view.FindViewById<CircularProgressIndicator>(Resource.Id.uploadIndicator);
        this.ProfilePicture = view.FindViewById<ShapeableImageView>(Resource.Id.profile_picture);
        this.SignOut = view.FindViewById<Button>(Resource.Id.logout);
        this.UsernameLayout = view.FindViewById<TextInputLayout>(Resource.Id.username_layout);
        this.UsernameInput = view.FindViewById<TextInputEditText>(Resource.Id.username);
        this.DisplayName = view.FindViewById<TextView>(Resource.Id.displayname_text);
        this.DeletePfP = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.delete);
        this.SelectPfP = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.library);
        this.CapturePfP = view.FindViewById<ExtendedFloatingActionButton>(Resource.Id.camera);
        this.SetUserInterfaceEnabled(false);
        this.SignOut!.Click += OnClickAccountAction;
        this.SelectPfP!.Click += OnClickAccountAction;
        this.CapturePfP!.Click += OnClickAccountAction;
        this.DeletePfP!.Click += OnClickAccountAction;
        this.UsernameInput!.SetImeActionLabel(nameof(ImeAction.Done), ImeAction.Done);
        this.UsernameInput.EditorAction += async (sender, args) =>
        {
            if (args.ActionId == ImeAction.Done) await this.OnUpdateUI(this.OnEditUsername(), this.DisplayName);
        };
        if (this.MainActivity?.Auth?.CurrentUser is not FirebaseUser user) // The user is not logged in
            this.SignInLauncher?.Launch(Common.User.GetSignInActivityIntent()); // Attempt to Sign In/Up the device
        else this.OnLoggedIn(user);
    }

    private void OnClickAccountAction(object? sender, EventArgs e)
    {
        if (sender is not View clicked) return;
        if (clicked.Id == this.SignOut?.Id) this.SignOutConfirmation?.Show();
        if (clicked.Id == this.DeletePfP?.Id) this.DeleteConfirmation?.Show();
        if (clicked.Id == this.SelectPfP?.Id) this.PhotoPicker?.Launch(Common.User.MediaRequest); // access the phones media
        if (clicked.Id == this.CapturePfP?.Id) this.PhotoTaker?.Launch(TemporarilyUploadToFile);  // access the phones camera
    }

    /// <summary> sets the contents of the views to match the user info and enables the ui </summary>
    private void OnLoggedIn(FirebaseUser user)
    {
        this.FirebaseUser = user;
        this.User = new Common.User(this.FirebaseUser);
        Glide.With(this).Load(this.User.ProfilePicture).SkipMemoryCache(true)
            .SetDiskCacheStrategy(DiskCacheStrategy.None!).Thumbnail(this.Thumbnail)
            .Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
        this.DisplayName!.Text = this.User.Client.Name;
        this.UsernameInput!.Hint = this.User.Client.Name;
        this.UsernameInput!.Text = this.User.Client.Name;
        this.SetUserInterfaceEnabled(true);
    }
    /// <summary> enables / disables all the buttons</summary>
    private void SetUserInterfaceEnabled(bool value)
    {
        this.SignOut!.Enabled = value;
        this.SelectPfP!.Enabled = value;
        this.CapturePfP!.Enabled = value;
        this.DeletePfP!.Enabled = value;
        this.UsernameLayout!.Enabled = value;
        this.UsernameInput!.Enabled = value;
        this.MainActivity!.NavigationBar!.Enabled = value;
    }
    /// <summary> tries to delete the users profile picture and Updates the users DisplayName in firebase </summary>
    private async Task OnEditUsername() => await this.FirebaseUser?.UpdateProfileAsync(this.ChangeUsernameRequest.Build())!;
    /// <param name="picked"> an Android.Net.Uri(aka AndroidUri) to the photo for uploading it</param>
    private async void PickPhoto(AndroidUri? picked) => await this.OnPickPhoto(picked);
    /// <summary> Uploads the picked photo to firebase and updates the user info, and ui </summary>
    private async Task OnPickPhoto(AndroidUri? picked)
    {
        if (picked is not null)
            await this.OnUpdateUI(this.User!.ProfilePicture.PutFile(picked).AsAsync(), this.SelectPfP);
    }
    /// <summary> converts the javaBoolean to a c#(.net) bool into OnTakePhoto to check if we should upload </summary>
    private async void TakePhoto(Java.Lang.Boolean? value) => await this.OnTakePhoto(value!.BooleanValue());
    /// <summary> Uploads the captured/taken photo if there is one</summary>
    private async Task OnTakePhoto(bool isTaken)
    {
        if (isTaken) await this.OnUpdateUI(this.User!.ProfilePicture.PutFile(this.TemporarilyUploadToFile!).AsAsync(), this.CapturePfP);
    }
    /// <summary> deletes the users profile picture / photo from firebase and updates the user info, and ui </summary>
    private async Task OnDeletePhoto() => await this.OnUpdateUI(this.User!.ProfilePicture.DeleteAsync(), this.DeletePfP);
    private async Task OnUpdateUI(Task action, object? sender)
    {
        if (sender is TextView displayName && displayName?.Id == this.DisplayName?.Id)
        {
            this.DisplayName!.Text = this.UsernameInput?.Text;
            this.UsernameInput!.Hint = this.DisplayName!.Text;
            this.SetUserInterfaceEnabled(false);
            await action;
            this.SetUserInterfaceEnabled(true);
            return;
        }

        if (sender is ExtendedFloatingActionButton button)
        {
            this.UploadIndicator!.Visibility = ViewStates.Visible;
            this.SetUserInterfaceEnabled(false);
            if ((this.DeletePfP?.Id) != button.Id || this.FirebaseUser?.PhotoUrl is not null)
                await this.OnUpdateProfilePicture(action, button);
            else button.Spin();
            this.SetUserInterfaceEnabled(true);
        }
    }

    private async Task OnUpdateProfilePicture(Task action, ExtendedFloatingActionButton ui)
    {
        await action;
        if (action.IsCompleted && this.UploadIndicator is not null)
            this.UploadIndicator!.Visibility = ViewStates.Invisible;

        if (action.IsCompletedSuccessfully)
        {
            try
            {
                ui.Spin();
                UserProfileChangeRequest.Builder request = new();
                if (ui.Id == this.DeletePfP?.Id)
                {
                    Glide.With(this).Clear(this.ProfilePicture!);
                    request = request.SetPhotoUri(null);
                }
                else
                {
                    request = request.SetPhotoUri(await this.User!.ProfilePicture.GetDownloadUrlAsync());
                    Glide.With(this).Load(this.User!.ProfilePicture).SetDiskCacheStrategy(DiskCacheStrategy.None!)
                        .SkipMemoryCache(true).Placeholder(this.ProfilePicture!.Drawable!).Into(this.ProfilePicture!);
                }
                await this.FirebaseUser?.UpdateProfileAsync(request.Build())!;
            } catch (Exception) { }
        }

        if (action.Exception is not null) ui.OnError(this.Activity!);
    }

    /// <summary>
    /// navigates the user back to the MainFragment in the case that the sign in/up operation fails to finish
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
                this.MainActivity!.NavigationBar!.SelectedItemId = Resource.Id.play;
                return;
            }
#pragma warning disable XAOBS001 // Type or member is obsolete
            if (response?.User?.PhotoUri is not null && response.IsNewUser)
            {
                var changeUserInfoRequest = new UserProfileChangeRequest.Builder().SetPhotoUri(null);
                await this.FirebaseUser?.UpdateProfileAsync(changeUserInfoRequest.Build())!;
            }
#pragma warning restore XAOBS001 // Type or member is obsolete
            this.OnLoggedIn(user);
            return;
        }
        if (response.Error?.ErrorCode == ErrorCodes.NoNetwork) //Sign in failed
        {
            Snackbar.Make(this.MainActivity!.FragmentContainer!, Resource.String.no_internet_connection, Snackbar.LengthLong);
            this.MainActivity!.NavigationBar!.SelectedItemId = Resource.Id.play;
            return;
        }
        Snackbar.Make(this.MainActivity!.FragmentContainer!, Resource.String.unknown_error, Snackbar.LengthLong).Show();
    }
}
