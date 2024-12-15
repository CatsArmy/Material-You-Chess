using Android.App;
using Firebase;
using Firebase.Auth;

namespace Chess.Firebase;

public class ChessFirebase : IFirebaseSecrets
{

    //"CreateUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //"LogInUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //No need to manualy set the current user
    //

    public readonly FirebaseAuth auth;
    public readonly FirebaseApp app;
    public readonly string TemplateEmail = IFirebaseSecrets.TemplateUserEmail;
    public readonly string TemplatePassword = IFirebaseSecrets.TemplateUserPassword;
    public ChessFirebase()
    {
        app = FirebaseApp.InitializeApp(Application.Context);
        if (app is null || app?.Options is null)
            app = FirebaseApp.InitializeApp(Application.Context,
                new FirebaseOptions.Builder()
                .SetApplicationId(IFirebaseSecrets.ApplicationId)
                .SetStorageBucket(IFirebaseSecrets.StorageBucket)
                .SetApiKey(IFirebaseSecrets.ApiKey)
                .SetProjectId(IFirebaseSecrets.ProjectId)
                .Build());

        auth = FirebaseAuth.Instance;
    }
}
