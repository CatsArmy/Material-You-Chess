using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Views;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using FirebaseUI.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Snackbar;

namespace Chess;

public class SelectAccountMethodFragment() : AndroidX.Fragment.App.Fragment(Resource.Layout.__profile_fragment__)
{
    public ActivityResultLauncher<Intent>? SignInLauncher;

    protected View? Root;

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.Root = view.RootView;
    }

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.SignInLauncher = this.RegisterForActivityResult<Intent, FirebaseAuthUIAuthenticationResult>(
            contract: new FirebaseAuthUIActivityResultContract(),
            callback: new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));

        if (FirebaseAuth.Instance.CurrentUser != null)
            return;

        this.OpenSignInIntentActivity();
    }

    private void OpenSignInIntentActivity()
    {
        List<AuthUI.IdpConfig> providers = [
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(false).Build(),
            new AuthUI.IdpConfig.GoogleBuilder().SetSignInOptions(GoogleSignInOptions.DefaultSignIn).Build()
        ];

        // Create and launch sign-in intent
        var signInIntent = new AuthUI.AuthIntent()
        {
            Providers = providers,
            Theme = Main.Instance!.ThemeId,
            Logo = Resource.Drawable.ic_launcher_foreground,
            LockOrientation = true,
            AlwaysShowProviderChoice = true,
        }.Build();

        // Attempt to Sign In/Up the device
        this.SignInLauncher?.Launch(signInIntent);
    }

    private void OnSignInResult(FirebaseAuthUIAuthenticationResult? result)
    {
        if (result is null)
        {
            Snackbar.Make(this.Root!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
            return;
        }

        var response = result.IdpResponse;
        this.HandleSignInResponse(result.ResultCode.IntValue(), response);
    }

    private void HandleSignInResponse(int resultCode, IdpResponse? response)
    {
        // Successfully signed in
        if (resultCode == ((int)Result.Ok))
        {
            //this.Frag
            return;
        }

        // Sign in failed
        if (response == null)
        {
            // User pressed back button
            Snackbar.Make(this.Root!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
            return;
        }

        (response.Error!.ErrorCode switch
        {
            ErrorCodes.NoNetwork => Snackbar.Make(this.Root!, Resource.String.no_internet_connection, Snackbar.LengthLong),
            ErrorCodes.ErrorUserDisabled => Snackbar.Make(this.Root!, Resource.String.account_disabled, Snackbar.LengthLong),
            _ => Snackbar.Make(this.Root!, Resource.String.unknown_error, Snackbar.LengthLong)
        }).Show();

        this.OpenSignInIntentActivity();
    }
}
