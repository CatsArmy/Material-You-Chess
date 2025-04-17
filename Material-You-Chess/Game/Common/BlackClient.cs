using System.Text.Json.Serialization;
using Bumptech.Glide;
using Chess.App.Common;

namespace Chess.Game.Common;

[method: JsonConstructor]
public class BlackClient(string Username, string Uid) : IPlayerClient
{
    [JsonInclude] public string Username { get; set; } = Username != null && Username != string.Empty ? Username : "Black Player";
    [JsonInclude] public string Uid { get; set; } = Uid;
    [JsonInclude] public bool IsWhite => false;
    private FirebaseUserClient GetFirebaseUserClient() => new(this.Username, this.Uid ?? throw IPlayerClient.NullUid);
    public RequestBuilder LoadProfilePicture(RequestManager glide) => this.GetFirebaseUserClient().LoadProfilePicture(glide);
    public RequestBuilder? TryLoadProfilePicture(RequestManager glide)
    {
        if (this.Uid is null || this.Uid == string.Empty) return null;
        try
        {
            return this.LoadProfilePicture(glide);
        }
        catch (Exception) { }
        return null;
    }
}
