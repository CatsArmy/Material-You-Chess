using System.Text.Json.Serialization;
using Chess.App.Common;

namespace Chess.Game.Common;

[method: JsonConstructor]
public class BlackClient(string? Username, string? Uid) : PlayerClient(Username ?? "Black Player", uid: Uid, isWhite: false)
{
    public BlackClient(FirebaseUserClient user) : this(user.Username, user.Uid) { }
}