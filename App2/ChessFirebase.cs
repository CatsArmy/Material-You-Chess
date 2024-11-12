using Android.App;
using Firebase;
using Firebase.Auth;

namespace Chess
{
    public class ChessFirebase
    {
        public readonly FirebaseAuth auth;
        public readonly FirebaseApp app;
        public ChessFirebase()
        {
            app = FirebaseApp.InitializeApp(Application.Context);
            if (app is null || app?.Options is null)
                app = FirebaseApp.InitializeApp(Application.Context,
                    new FirebaseOptions.Builder().SetApplicationId("1:64933668749:android:a3d6ec6c10a3fe48837fc4")
                    .SetStorageBucket("material-you-chess.firebasestorage.app")
                    .SetApiKey("AIzaSyAtqSeZJ2_E11DL1xq5JS1AKd5n7jHE2Xk")
                    .SetProjectId("material-you-chess")
                    .Build());

            auth = FirebaseAuth.Instance;
        }
    }
}