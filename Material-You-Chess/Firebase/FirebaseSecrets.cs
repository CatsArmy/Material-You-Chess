using Firebase;

namespace Chess.Firebase;

public class FirebaseSecrets : IFirebaseSecrets
{

    //"CreateUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //"LogInUserWithEmailAndPassword auto signs in the user: auth.UpdateCurrentUser()"
    //No need to manually set the current user

    public static FirebaseSecrets? Instance { get; private set; } = null;
    public readonly FirebaseApp app;
    public readonly string TemplateEmail = IFirebaseSecrets.TemplateUserEmail;
    public readonly string TemplatePassword = IFirebaseSecrets.TemplateUserPassword;
    public FirebaseSecrets()
    {
        if (Instance != null)
        {
            this.app = Instance.app;
            return;
        }
        app = global::Firebase.FirebaseApp.InitializeApp(Application.Context,
            new FirebaseOptions.Builder()
            .SetApplicationId(IFirebaseSecrets.ApplicationId)
            .SetStorageBucket(IFirebaseSecrets.StorageBucket)
            .SetApiKey(IFirebaseSecrets.ApiKey)
            .SetProjectId(IFirebaseSecrets.ProjectId)
            .Build());
        Instance = this;
    }
}
