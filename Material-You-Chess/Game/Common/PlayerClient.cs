using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.App.Common;
using static Chess.App.Common.IFirebaseUser;

namespace Chess.Game.Common;

[JsonPolymorphic()]
[JsonDerivedType(typeof(PlayerClient), nameof(PlayerClient))]
[JsonDerivedType(typeof(WhiteClient), nameof(WhiteClient))]
[JsonDerivedType(typeof(BlackClient), nameof(BlackClient))]

[method: JsonConstructor]
public class PlayerClient(string username, string? uid = null, bool? isWhite = null) : IFirebaseUser
{
    [JsonInclude] public readonly string Username = username;
    [JsonInclude] public readonly string? Uid = uid;
    [JsonInclude] public bool? IsWhite = isWhite;

    public PlayerClient(FirebaseUserClient user) : this(user.Username, user.Uid, null) { }
    public RequestBuilder? TryLoadProfilePicture(RequestManager glide)
    {
        if (this.Uid is null) return null;

        try
        {
            return this.LoadProfilePicture(glide);
        }
        catch (Exception) { }

        return null;
    }

    public RequestBuilder LoadProfilePicture(RequestManager glide)
    {
        return new FirebaseUserClient(this.Username, this.Uid ?? throw NullUid).LoadProfilePicture(glide);
    }
}