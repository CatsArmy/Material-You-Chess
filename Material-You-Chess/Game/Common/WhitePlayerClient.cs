using Chess.App.Common;
using Firebase.Auth;

namespace Chess.Game.Common;

public class WhitePlayerClient(string Username, string Uid) : FirebaseUserClient(Username, Uid)
{
    public WhitePlayerClient(FirebaseUser user) : this(user.DisplayName ?? "White Player", user.Uid) { }
}
