using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.Game.Board;
using Chess.Game.Moves;
using Firebase.Storage;

namespace Chess.App.Networked;

[JsonSerializable(typeof(PlayerClient))]
[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(SerializedType))]
[JsonSourceGenerationOptions(IncludeFields = true, WriteIndented = true)]
internal partial class SourceJsonGenerationContext : JsonSerializerContext;

[JsonPolymorphic()]
[JsonDerivedType(typeof(PlayerClient), nameof(PlayerClient))]
[JsonDerivedType(typeof(WhiteClient), nameof(WhiteClient))]
[JsonDerivedType(typeof(BlackClient), nameof(BlackClient))]
public class PlayerClient(string uid)
{
    public string Uid = uid;

    public RequestBuilder LoadProfilePicture(RequestManager glide)
        => glide.Load(FirebaseStorage.Instance.Reference.Child($"{this.Uid}.png"))
        .Error(Resource.Drawable.outline_account_circle_24);
}

public class WhiteClient(string uid, string username) : PlayerClient(uid)
{
    public string WhitePlayerName = username;
}

public class BlackClient(string uid, string username) : PlayerClient(uid)
{
    public string BlackPlayerName = username;
}
