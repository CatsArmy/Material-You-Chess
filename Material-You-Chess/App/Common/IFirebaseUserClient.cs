using System.Text.Json.Serialization;
using Bumptech.Glide;

namespace Chess.App.Common;

public interface IFirebaseUserClient
{
    [JsonInclude] public string Username { get; }
    [JsonInclude] public string Uid { get; }

    public static readonly NullReferenceException NullUsername = new("Missing display name");
    public static readonly NullReferenceException NullUid = new("Missing Uid");
    public RequestBuilder LoadProfilePicture(RequestManager glide);
}