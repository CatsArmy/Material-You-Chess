using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.App.Common;
using Firebase.Auth;
using Firebase.Storage;

namespace Chess.App.Networked;

[JsonPolymorphic()]
[JsonDerivedType(typeof(WhitePlayerClient), nameof(WhitePlayerClient))]
[JsonDerivedType(typeof(BlackPlayerClient), nameof(BlackPlayerClient))]
[JsonDerivedType(typeof(FirebaseUserClient), nameof(FirebaseUserClient))]
public class FirebaseUserClient(string Username, string Uid) : UserClient(Username)
{
    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName ?? throw new("Missing display name"), user.Uid) { }
    public readonly string Uid = Uid;

    public StorageReference UserDir => FirebaseStorage.Instance.GetReference($"users/{this.Uid}/");
    public StorageReference ImagesDir => this.UserDir.Child($"images/");
    public StorageReference ProfilePicture => this.ImagesDir.Child($"user.image");

    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(this.ProfilePicture).Error(Resource.Drawable.outline_account_circle_24);
}
