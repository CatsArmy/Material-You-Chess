using Bumptech.Glide;
using Chess.Game.Common;
using Firebase.Auth;
using Firebase.Storage;
using static Chess.App.Common.IFirebaseUserClient;

namespace Chess.App.Common;

public class FirebaseUserClient(string Username, string Uid) : IFirebaseUserClient
{
    public readonly string Username = Username;
    public readonly string Uid = Uid;

    public StorageReference ProfilePicture => this.ImagesDir.Child($"user.image");
    public StorageReference ImagesDir => this.UserDir.Child($"images/");
    public StorageReference UserDir => FirebaseStorage.Instance.GetReference($"users/{this.Uid}/");

    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName ?? throw NullUsername, user.Uid) { }
    public FirebaseUserClient(PlayerClient user) : this(user.Username ?? throw NullUsername, user.Uid ?? throw NullUid) { }
    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(ProfilePicture).Error(Resource.Drawable.account_circle);
}
