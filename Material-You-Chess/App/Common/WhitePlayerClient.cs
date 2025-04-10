using Firebase.Auth;

namespace Chess.App.Common;

public class WhitePlayerClient(string Username, string Uid) : FirebaseUserClient(Username, Uid)
{
    public WhitePlayerClient(FirebaseUser user) : this(user.DisplayName ?? "White Player", user.Uid) { }
}
