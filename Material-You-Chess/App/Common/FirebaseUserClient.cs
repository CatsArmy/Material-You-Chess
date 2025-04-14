using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.Game.Common;
using Firebase.Auth;
using Firebase.Storage;
using static Chess.App.Common.IFirebaseUser;

namespace Chess.App.Common;

[method: JsonConstructor]
public class FirebaseUserClient(string Username, string Uid) : IFirebaseUser
{
    [JsonInclude] public readonly string Username = Username;
    [JsonInclude] public readonly string Uid = Uid;

    [JsonIgnore] public StorageReference ProfilePicture => this.ImagesDir.Child($"user.image");
    [JsonIgnore] public StorageReference ImagesDir => this.UserDir.Child($"images/");
    [JsonIgnore] public StorageReference UserDir => FirebaseStorage.Instance.GetReference($"users/{this.Uid}/");

    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName ?? throw NullUsername, user.Uid) { }
    public FirebaseUserClient(PlayerClient user) : this(user.Username ?? throw NullUsername, user.Uid ?? throw NullUid) { }
    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(ProfilePicture).Error(Resource.Drawable.account_circle);
}
