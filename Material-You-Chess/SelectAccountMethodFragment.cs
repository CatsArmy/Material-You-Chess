using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.Credentials;
using Chess.App.Common;
using Chess.App.Common.ActivityResult;
using Firebase.Auth;
using FirebaseUI.Auth;
using FirebaseUI.Auth.Data.Model;
using Google.Android.Material.Snackbar;
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
        this.SignInLauncher = this.RegisterForActivityResult<Intent, FirebaseAuthUIAuthenticationResult>(
            new FirebaseAuthUIActivityResultContract(),
            new ActivityResultCallback<FirebaseAuthUIAuthenticationResult>(this.OnSignInResult));

        this.OpenSignInIntentActivity();
    }

    private async void OpenSignInIntentActivity()
    {
        var googleIdOption = new GetGoogleIdOption.Builder()
            .SetFilterByAuthorizedAccounts(true)
            .SetAutoSelectEnabled(true)
            //.SetNonce(Guid.NewGuid().ToString())
            .Build();
        var request = new GetCredentialRequest.Builder().AddCredentialOption(googleIdOption).Build();
        var credentialManager = CredentialManager.Create(this.Context!);

        await credentialManager.GetCredentialAsync(request, new CancellationToken());

        List<AuthUI.IdpConfig> providers =
            [new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
            new AuthUI.IdpConfig.GoogleBuilder().SetSignInOptions(GoogleSignInOptions.DefaultSignIn).Build()
            ];

        // Create and launch sign-in intent
        var signInIntent = new AuthUI.AuthIntent()
        {
            Providers = providers,
            Theme = Main.Instance!.ThemeId,
            Logo = Resource.Drawable.ic_launcher_foreground,
            LockOrientation = true,
        }.Build();

        if (FirebaseAuth.Instance.CurrentUser != null)
            return;

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
            this.ParentFragmentManager?.BeginTransaction()?.SetReorderingAllowed(true)
                ?.Replace(Resource.Id.fragment_container_view, new ProfileFragment())?.Commit();
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

    public void handleSignIn(GetCredentialResponse result)
    {
        // Handle the successfully returned credential.
        var credential = result.Credential;
        // Passkey credential
        if (credential is PublicKeyCredential passkey)
        {
            // Share responseJson such as a GetCredentialResponse on your server to
            // validate and authenticate
            responseJson = passkey.AuthenticationResponseJson;
        }

        // Password credential
        if (credential is PasswordCredential cred)
        {
            // Send ID and password to your server to validate and authenticate.
            var username = cred.Id;
            var password = cred.Password;

        }

        // GoogleIdToken credential
        if (credential is not CustomCredential)
        {
            // Catch any unrecognized credential type here.
            Logger.Error("Unexpected type of credential");
            return;
        }

        if (credential.Type == GoogleIdTokenCredential.TypeGoogleIdTokenCredential)
        {
            try
            {
                // Use googleIdTokenCredential and extract the ID to validate and
                // authenticate on your server.
                var googleIdTokenCredential = GoogleIdTokenCredential.CreateFrom(credential.Data);
                // You can use the members of googleIdTokenCredential directly for UX
                // purposes, but don't use them to store or control access to user
                // data. For that you first need to validate the token:
                // pass googleIdTokenCredential.getIdToken() to the backend server.
                //GoogleIdTokenVerifier
                //var verifier = ... // see validation instructions

                GoogleIdTokenCredential idToken = verifier.verify(googleIdTokenCredential.IdToken);
                // To get a stable account identifier (e.g. for storing user data),
                // use the subject ID:
                idToken.getPayload().getSubject()
                    }
            catch (e: GoogleIdTokenParsingException) {
                Log.e(TAG, "Received an invalid google id token response", e)
        }
            } else
            {
                // Catch any unrecognized custom credential type here.
                Log.e(TAG, "Unexpected type of credential")
                    }
        }

        else
        {

        }
    }
}

internal sealed class CredentialManagerCallback<TResult, TException>(CancellationToken cancellationToken)
    : CallbackBase<TResult, TException>(cancellationToken)
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
    : CallbackBase<TException>(cancellationToken), ICredentialManagerCallback
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

internal abstract class CallbackBase<TResult, TException> : Java.Lang.Object
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource<TResult?> _taskCompletionSource;

    public CallbackBase(CancellationToken cancellationToken)
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

internal abstract class CallbackBase<TException> : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource _taskCompletionSource;

    public CallbackBase(CancellationToken cancellationToken)
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
