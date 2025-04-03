using Firebase.Auth;

namespace Chess.App.Networked;

public class WhitePlayerClient(string Username, string Uid) : FirebaseUserClient(Username, Uid)
{
    public WhitePlayerClient(FirebaseUser user) : this(user.DisplayName ?? "White Player", user.Uid) { }
}
