using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.App.Common;

namespace Chess.Game.Common;

[JsonPolymorphic()]
[JsonDerivedType(typeof(IPlayerClient), nameof(IPlayerClient))]
[JsonDerivedType(typeof(WhiteClient), nameof(WhiteClient))]
[JsonDerivedType(typeof(BlackClient), nameof(BlackClient))]
public interface IPlayerClient : IFirebaseUserClient
{
    [JsonInclude] public bool IsWhite { get; }
    public RequestBuilder? TryLoadProfilePicture(RequestManager glide);
}