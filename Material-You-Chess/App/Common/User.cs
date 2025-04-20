using System.Text.Json.Serialization;
using Android.Content;
using AndroidX.Activity.Result;
using Bumptech.Glide;
using Firebase.Auth;
using Firebase.Storage;
using FirebaseUI.Auth;
using static AndroidX.Activity.Result.Contract.ActivityResultContracts;

namespace Material.You.Chess.App.Common;

[method: JsonConstructor]
public record class User([property: JsonInclude] Client Client, [property: JsonInclude] string Uid)
{
    public User(FirebaseUser user) : this(new(user.DisplayName ?? throw NullUsername), user.Uid) { }
    [JsonIgnore] public static readonly NullReferenceException NullUsername = new("Missing display name");
    [JsonIgnore] public static readonly NullReferenceException NullUid = new("Missing Uid");
    [JsonIgnore] private static PickVisualMedia.IVisualMediaType Media => PickVisualMedia.ImageOnly.Instance;
    [JsonIgnore] public static PickVisualMediaRequest MediaRequest => new PickVisualMediaRequest.Builder().SetMediaType(Media).Build();
    [JsonIgnore] public StorageReference Users => FirebaseStorage.Instance.GetReference(location: $"{nameof(Users).ToLower()}/{Uid}/");
    [JsonIgnore] public StorageReference Images => this.Users.Child(pathString: $"{nameof(Images).ToLower()}/");
    [JsonIgnore] public StorageReference ProfilePicture => this.Images.Child(pathString: $"{nameof(User).ToLower()}.image");
    public RequestBuilder LoadPfP(RequestManager glide) => glide.Load(ProfilePicture);
    public RequestBuilder? TryLoadPfP(RequestManager? glide)
    {
        if (glide is null || this.Uid is null || this.Uid == string.Empty) return null;
        try
        {
            return this.LoadPfP(glide).Error(Resource.Drawable.account_circle);
        } catch (Exception) { }

        return null;
    }

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
