using Android.Content;
using Android.Views;
using AndroidX.Credentials;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.TextField;
using Java.Lang;

namespace Chess.App.Credentials;

public class SignInFragment : AndroidX.Fragment.App.Fragment
{

    private ICredentialManager? credentialManager;
    private SignInFragmentCallback? listener;
    private Button? signInWithSavedCredentials;
    private TextInputEditText? text_usernameEditText;
    private TextInputLayout? text_usernameLayout;
    private CircularProgressIndicator? circularProgressIndicator;
    private TextView? textProgress;

    public override void OnAttach(Context context)
    {
        base.OnAttach(context);
        try
        {
            listener = context as SignInFragmentCallback;
        }
        // The activity does not implement the listener. 
        catch (ClassCastException) { }
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
    => inflater.Inflate(Resource.Layout.fragment_sign_in, container, false)!;

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        credentialManager = CredentialManager.Create(RequireActivity());

        var getCredentialRequest = configureGetCredentialRequest();

        configureAutofill(getCredentialRequest);

        this.signInWithSavedCredentials!.Click += (_, _) => SignInWithSavedCredentials(getCredentialRequest);
    }

    private void configureAutofill(GetCredentialRequest getCredentialRequest)
    {
        this.text_usernameEditText.PendingCredentialCallback = pendingGetCredentialRequest(getCredentialRequest) {
            response =>
            if (response.credential is PublicKeyCredential)
            {
                DataProvider.setSignedInThroughPasskeys(true)
            }
            if (response.credential is PasswordCredential)
            {
                DataProvider.setSignedInThroughPasskeys(false)
            }
            showHome()
        }
    }

    private GetCredentialRequest configureGetCredentialRequest()
    {
        var getPublicKeyCredentialOption = new GetPublicKeyCredentialOption(fetchAuthJsonFromServer(), null);
        var getPasswordOption = GetPasswordOption()
        var getCredentialRequest = new GetCredentialRequest([getPublicKeyCredentialOption, getPasswordOption]);
        return getCredentialRequest
    }

    private void SignInWithSavedCredentials(GetCredentialRequest getCredentialRequest)
    {
        configureViews(ViewStates.Invisible, false);


        //data = getSavedCredentials(getCredentialRequest);


        configureViews(ViewStates.Invisible, true);


        //data?.let
        //{ showHome()}


    }



    private void showHome()
    {
        sendSignInResponseToServer();
        listener?.showHome();
    }

    private void configureViews(ViewStates visibility, bool flag)
    {
        configureProgress(visibility);
        signInWithSavedCredentials!.Enabled = flag;
    }

    private void configureProgress(ViewStates visibility)
    {
        textProgress!.Visibility = visibility;
        circularProgressIndicator!.Visibility = visibility;
    }

    private string fetchAuthJsonFromServer()
    {
        return RequireContext().readFromAsset("AuthFromServer");
    }

    private bool sendSignInResponseToServer()
    {
        return true;
    }

    private string? getSavedCredentials(GetCredentialRequest getCredentialRequest)
    {
        ICredentialManager? result;
        try
        {
            result = credentialManager!.GetCredential(RequireActivity(), getCredentialRequest, null) as ICredentialManager;
        }
        catch (System.Exception)
        {
            configureViews(ViewStates.Invisible, true);
            //Log.e("Auth", "getCredential failed with exception: " + e.message.toString())
            this.Activity?.showErrorAlert("An error occurred while authenticating through saved credentials. Check logs for additional details");
            return null;
        }

        if (result.Credential is PublicKeyCredential)
        {
            var cred = result.credential as PublicKeyCredential;
            DataProvider.setSignedInThroughPasskeys(true);
            return "Passkey: ${cred.authenticationResponseJson}";
        }
        if (result.credential is PasswordCredential)
        {
            var cred = result.credential as PasswordCredential;
            DataProvider.setSignedInThroughPasskeys(false);
            return "Got Password - User:${cred.id} Password: ${cred.password}";
        }
        if (result.credential is CustomCredential)
        {
            //If you are also using any external sign-in libraries, parse them here with the
            // utility functions provided.
        }
        return null;
    }

    public override void OnDestroyView()
    {
        base.OnDestroyView();
        configureProgress(View.INVISIBLE);
        _binding = null;
    }

}

public interface SignInFragmentCallback
{
    void showHome();
}