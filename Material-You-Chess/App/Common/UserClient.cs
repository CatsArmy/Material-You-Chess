using System.Text.Json.Serialization;
using Bumptech.Glide;
using Firebase.Storage;

namespace Chess.App.Common;

[JsonPolymorphic()]
[JsonDerivedType(typeof(WhiteClient), nameof(WhiteClient))]
[JsonDerivedType(typeof(BlackClient), nameof(BlackClient))]
[JsonDerivedType(typeof(UserClient), nameof(UserClient))]
public class UserClient(string? Uid, string? Username = null)
{
    public string? Username = Username;
    public string? Uid = Uid;

    public RequestBuilder LoadProfilePicture(RequestManager glide)
        => glide.Load(FirebaseStorage.Instance.Reference.Child($"{Uid}"))
        .Error(Resource.Drawable.outline_account_circle_24);

    public RequestBuilder DownloadProfilePicture(RequestManager glide)
        => glide.Download(FirebaseStorage.Instance.Reference.Child($"{Uid}"))
        .Error(Resource.Drawable.outline_account_circle_24);
}

public class WhiteClient(string Uid, string WhitePlayerName) : UserClient(Uid, WhitePlayerName)
{
    public string WhitePlayerName = WhitePlayerName;
}

public class BlackClient(string Uid, string BlackPlayerName) : UserClient(Uid, BlackPlayerName)
{
    public string BlackPlayerName = BlackPlayerName;
}
