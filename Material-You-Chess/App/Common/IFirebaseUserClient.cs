using Bumptech.Glide;

namespace Chess.App.Common;

public interface IFirebaseUserClient
{
    public static readonly NullReferenceException NullUsername = new("Missing display name");
    public static readonly NullReferenceException NullUid = new("Missing Uid");
    public RequestBuilder LoadProfilePicture(RequestManager glide);
}