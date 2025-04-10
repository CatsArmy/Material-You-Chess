using Firebase.Auth;

namespace Chess.App.Common;

public class BlackPlayerClient(string Username, string Uid) : FirebaseUserClient(Username, Uid)
{
    public BlackPlayerClient(FirebaseUser user) : this(user.DisplayName ?? "Black Player", user.Uid) { }
}