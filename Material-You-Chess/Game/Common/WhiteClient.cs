using System.Text.Json.Serialization;
using Chess.App.Common;

namespace Chess.Game.Common;

[method: JsonConstructor]
public class WhiteClient(string? username, string? Uid) : PlayerClient(username ?? "White Player", uid: Uid, isWhite: true)
{
    public WhiteClient(FirebaseUserClient user) : this(user.Username, user.Uid) { }
}
