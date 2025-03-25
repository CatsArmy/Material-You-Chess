using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.App.Common;
using Chess.Game.Board;
using Chess.Game.Moves;
using Firebase.Auth;
using Firebase.Storage;

namespace Chess.App.Networked;

[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(FirebaseUserClient))]
[JsonSerializable(typeof(SerializedType))]
#if DEBUG
[JsonSourceGenerationOptions(IncludeFields = true, WriteIndented = true)]
#else
[JsonSourceGenerationOptions(IncludeFields = true)]
#endif
internal partial class SourceJsonGenerationContext : JsonSerializerContext;

[JsonPolymorphic()]
[JsonDerivedType(typeof(WhitePlayerClient), nameof(WhitePlayerClient))]
[JsonDerivedType(typeof(BlackPlayerClient), nameof(BlackPlayerClient))]
[JsonDerivedType(typeof(FirebaseUserClient), nameof(FirebaseUserClient))]
public class FirebaseUserClient(string Username, string? Uid) : UserClient(Username)
{
    public FirebaseUserClient(FirebaseUser user) : this(user.DisplayName!, user.Uid) { }
    public readonly string? Uid = Uid;

    public RequestBuilder LoadProfilePicture(RequestManager glide) =>
        glide.Load(FirebaseStorage.Instance.GetReference(this.Uid!))
        .Error(Resource.Drawable.outline_account_circle_24);
}

public class WhitePlayerClient(string Username, string? Uid) : FirebaseUserClient(Username, Uid)
{
    public WhitePlayerClient(FirebaseUser user) : this(user.DisplayName!, user.Uid) { }
}

public class BlackPlayerClient(string Username, string? Uid) : FirebaseUserClient(Username, Uid)
{
    public BlackPlayerClient(FirebaseUser user) : this(user.DisplayName!, user.Uid) { }
}