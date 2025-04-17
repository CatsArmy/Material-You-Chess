using Android.Content;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Chess.Game.Common;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseUI.Auth;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;
using static Chess.App.Common.IFirebaseUserClient;

namespace Chess.App.Common;

public class FirebaseUserClient(string username, string uid) : IFirebaseUserClient
{
    public string Username => username;
    public string Uid => uid;

    private PickVisualMedia.IVisualMediaType Type => PickVisualMedia.ImageOnly.Instance;
    public PickVisualMediaRequest Request => new PickVisualMediaRequest.Builder().SetMediaType(this.Type).Build();
    public StorageReference ProfilePicture => this.ImagesDir.Child($"user.image");
    public StorageReference ImagesDir => this.UserDir.Child($"images/");
    public StorageReference UserDir => FirebaseStorage.Instance.GetReference($"users/{this.Uid}/");

    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName ?? throw NullUsername, user.Uid) { }
    public FirebaseUserClient(IPlayerClient user) : this(user.Username ?? throw NullUsername, user.Uid ?? throw NullUid) { }
    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(ProfilePicture).Error(Resource.Drawable.account_circle);

    /// <summary>
    /// returns a customized Sign Up/In activity intent from the NuGet Package: "FirebaseUI" 
    /// and sets the available sign-in providers to be either: sign-in via email or sign-in via a private google account
    /// </summary>
    public static Intent GetSignInActivityIntent()
    {
        List<AuthUI.IdpConfig> providers = [
            new AuthUI.IdpConfig.GoogleBuilder().Build(),
            new AuthUI.IdpConfig.EmailBuilder().SetRequireName(true).SetAllowNewAccounts(true).Build(),
        ];

        // Create and launch sign-in intent
        var signInIntentBuilder = AuthUI.Instance.CreateSignInIntentBuilder();
        signInIntentBuilder.SetAvailableProviders(providers);
        //Customize the FirebaseUI-Auth sign-in/up screen
        signInIntentBuilder.SetTheme(Resource.Style.AppTheme_Material3_DynamicColors_DayNight_NoActionBar);
        signInIntentBuilder.SetLogo(Resource.Drawable.ic_launcher_foreground);
        signInIntentBuilder.SetCredentialManagerEnabled(true);
        signInIntentBuilder.SetLockOrientation(true);
        //Build the activities intent and return it ready for use
        return signInIntentBuilder.Build();
    }
}
