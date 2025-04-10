using System.Text.Json.Serialization;
using Bumptech.Glide;
using Firebase.Auth;
using Firebase.Storage;

namespace Chess.App.Common;

[JsonPolymorphic()]
[JsonDerivedType(typeof(WhitePlayerClient), nameof(WhitePlayerClient))]
[JsonDerivedType(typeof(BlackPlayerClient), nameof(BlackPlayerClient))]
[JsonDerivedType(typeof(FirebaseUserClient), nameof(FirebaseUserClient))]
public class FirebaseUserClient(string Username, string Uid) : UserClient(Username)
{
    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName ?? throw new("Missing display name"), user.Uid) { }
    public readonly string Uid = Uid;

    public StorageReference UserDir => FirebaseStorage.Instance.GetReference($"users/{Uid}/");
    public StorageReference ImagesDir => UserDir.Child($"images/");
    public StorageReference ProfilePicture => ImagesDir.Child($"user.image");

    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(ProfilePicture).Error(Resource.Drawable.outline_account_circle_24);
}
