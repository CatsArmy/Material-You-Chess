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

    public StorageReference ProfilePicture => FirebaseStorage.Instance.GetReference($"user/{this.Uid}.image");

    public RequestBuilder LoadProfilePicture(RequestManager glide) => glide.Load(this.ProfilePicture).Error(Resource.Drawable.outline_account_circle_24);
}
