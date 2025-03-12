using Android.Content;
using Android.Runtime;
using Android.Views;
using AndroidX.Credentials;
using AndroidX.Credentials.Exceptions;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;

namespace Chess;

public class SelectAccountMethodFragment() : AndroidX.Fragment.App.Fragment()
{
    private View? Root;

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => this.Root = inflater.Inflate(Resource.Layout.__profile_fragment__, container, false);

    public ActivityResultLauncher<Intent>? SignInLauncher;

    public override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        //this.SignInLauncher = this.RegisterForActivityResult<Intent, FirebaseAuthUIAuthenticationResult>(
        //    new FirebaseAuthUIActivityResultContract(),
        //    new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));
        if (FirebaseAuth.Instance.CurrentUser != null)
        {
            Logger.Debug("the device is signed in");
            try
            {
                Logger.Debug(FirebaseAuth.Instance.CurrentUser.DisplayName!);
            }
            catch (Exception e)
            {
                Logger.Warn($"{e}");
            }
            return;
        }
        this.OpenSignInIntentActivity();
    }

    private async void OpenSignInIntentActivity()
    {
        var credentialManager = CredentialManager.Create(this.Context!);

        var googleIdOption = new GetGoogleIdOption.Builder()
            .SetNonce($"{Guid.NewGuid()}")
            .SetServerClientId(base.GetString(Resource.String.default_web_client_id))
            .SetFilterByAuthorizedAccounts(true)
            //.SetAutoSelectEnabled(false)
            .Build();
        var signInWithGoogle = new GetSignInWithGoogleOption.Builder(
            base.GetString(Resource.String.default_web_client_id)).Build();

        //var requestIdOptions = new GetCredentialRequest.Builder().AddCredentialOption(googleIdOption).Build();

        var requestSignInWithGoogle = new GetCredentialRequest.Builder().AddCredentialOption(signInWithGoogle).Build();
        try
        {
            //var result = await credentialManager.GetCredentialAsync(requestIdOptions, new());
            var result = await credentialManager.GetCredentialAsync(requestSignInWithGoogle, new());
            await HandleSignIn(result);
        }
        catch (GetCredentialException e)
        {
            Logger.Warn($"credential error: {e}");
        }


        //List<AuthUI.IdpConfig> providers =
        //    [new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(false).Build(),
        //    new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
        //    //new AuthUI.IdpConfig.GoogleBuilder().SetSignInOptions(GoogleSignInOptions.DefaultSignIn).Build()
        //    ];

        //// Create and launch sign-in intent
        //var signInIntent = new AuthUI.AuthIntent()
        //{
        //    Providers = providers,
        //    Theme = Main.Instance!.ThemeId,
        //    Logo = Resource.Drawable.ic_launcher_foreground,
        //    LockOrientation = true,
        //}.Build();

        // Attempt to Sign In/Up the device
        //this.SignInLauncher?.Launch(signInIntent);
    }

    //private void OnSignInResult(FirebaseAuthUIAuthenticationResult? result)
    //{
    //    if (result is null)
    //    {
    //        Snackbar.Make(this.Root!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
    //        return;
    //    }

    //    var response = result.IdpResponse;
    //    this.HandleSignInResponse(result.ResultCode.IntValue(), response);
    //}

    //private void HandleSignInResponse(int resultCode, IdpResponse? response)
    //{
    //    // Successfully signed in
    //    if (resultCode == ((int)Result.Ok))
    //    {
    //        this.ParentFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
    //            ?.Replace(Resource.Id.fragment_container_view, new ProfileFragment())?.Commit();
    //        return;
    //    }

    //    // Sign in failed
    //    if (response == null)
    //    {
    //        // User pressed back button
    //        Snackbar.Make(this.Root!, Resource.String.sign_in_cancelled, Snackbar.LengthLong).Show();
    //        return;
    //    }

    //    (response.Error!.ErrorCode switch
    //    {
    //        ErrorCodes.NoNetwork => Snackbar.Make(this.Root!, Resource.String.no_internet_connection, Snackbar.LengthLong),
    //        ErrorCodes.ErrorUserDisabled => Snackbar.Make(this.Root!, Resource.String.account_disabled, Snackbar.LengthLong),
    //        _ => Snackbar.Make(this.Root!, Resource.String.unknown_error, Snackbar.LengthLong)
    //    }).Show();

    //    this.OpenSignInIntentActivity();
    //}

    public async Task HandleSignIn(GetCredentialResponse? result)
    {
        // Handle the successfully returned credential.
        var credential = result?.Credential;

        if (credential is not CustomCredential) // GoogleIdToken credential
        {
            // Catch any unrecognized credential type here.
            Logger.Error("Unexpected type of credential");
            return;
        }

        if (credential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
        {
            try
            {
                var googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(result!.Credential.Data);

                // Sign in to Firebase with using the token
                var auth = GoogleAuthProvider.GetCredential(googleIdTokenCredential.IdToken, null);
                var signInTask = FirebaseAuth.Instance.SignInWithCredentialAsync(auth);
                var authResult = await signInTask;

                if (signInTask.IsCompletedSuccessfully)
                {
                    // Sign in success, update UI with the signed-in user's information
                    Logger.Debug("signInWithCredential:success");
                    var user = FirebaseAuth.Instance.CurrentUser;
                    Logger.Debug($"{user?.DisplayName}");
                    return;
                }

                if (signInTask.IsFaulted)
                {
                    // If sign in fails, display a message to the user
                    Logger.Warn($"signInWithCredential:failure {signInTask.Exception}");
                }

                Logger.Warn($"signInWithCredential:failure");
                //updateUI(null);
            }
            catch (GoogleIdTokenParsingException e)
            {
                Logger.Error($"Received an invalid google id token response {e}");
            }
            return;
        }

        if (credential is PublicKeyCredential passkeyCredential) // Passkey credential
        {
            // Share responseJson such as a GetCredentialResponse on your server to validate and authenticate

            //var responseJson = passkeyCredential.AuthenticationResponseJson;
            //return;
        }

        if (credential is PasswordCredential passwordCredential) // Password credential
        {
            // Send ID and password to your server to validate and authenticate.

            //var username = passwordCredential.Id;
            //var password = passwordCredential.Password;
            //var auth = EmailAuthProvider.GetCredential(username, password);
            //var signInTask = FirebaseAuth.Instance.SignInWithCredentialAsync(auth);
            //return;
        }



        // Catch any unrecognized custom credential type here.
        Logger.Error("Unexpected type of credential");
    }
}


#region

internal sealed class CredentialManagerCallback<TResult, TException>(CancellationToken cancellationToken)
    : AsyncCallback<TResult, TException>(cancellationToken)
    , ICredentialManagerCallback
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    public void OnResult(Java.Lang.Object? result)
    {
        var parsedResult = result is not null
            ? (TResult)result
            : null;

        ReportSuccess(parsedResult);
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}

internal class CredentialManagerCallback<TException>(CancellationToken cancellationToken)
    : AsyncCallback<TException>(cancellationToken)
    , ICredentialManagerCallback
    where TException : Java.Lang.Exception
{
    public void OnResult(Java.Lang.Object? result)
    {
        ReportSuccess();
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}

internal abstract class AsyncCallback<TResult, TException> : Java.Lang.Object
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource<TResult?> _taskCompletionSource;

    public AsyncCallback(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<TResult?>();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task<TResult?> Task => _taskCompletionSource.Task;

    protected void ReportSuccess(TResult? result)
    {
        _taskCompletionSource.TrySetResult(result);
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}

internal abstract class AsyncCallback<TException> : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource _taskCompletionSource;

    public AsyncCallback(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task Task => _taskCompletionSource.Task;

    protected void ReportSuccess()
    {
        _taskCompletionSource.TrySetResult();
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}
#endregion