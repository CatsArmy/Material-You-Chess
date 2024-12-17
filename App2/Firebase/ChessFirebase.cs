using Android.App;
using Firebase;

namespace Chess.FirebaseApp;

public class ChessFirebase : IFirebaseSecrets
{

    //"CreateUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //"LogInUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //No need to manualy set the current user
    //

    public readonly Firebase.FirebaseApp app;
    public readonly string TemplateEmail = IFirebaseSecrets.TemplateUserEmail;
    public readonly string TemplatePassword = IFirebaseSecrets.TemplateUserPassword;
    public ChessFirebase()
    {
        app = Firebase.FirebaseApp.InitializeApp(Application.Context,
            new FirebaseOptions.Builder()
            .SetApplicationId(IFirebaseSecrets.ApplicationId)
            .SetStorageBucket(IFirebaseSecrets.StorageBucket)
            .SetApiKey(IFirebaseSecrets.ApiKey)
            .SetProjectId(IFirebaseSecrets.ProjectId)
            .Build());
    }
}
