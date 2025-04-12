using Chess.App.Common;
using Firebase.Auth;

namespace Chess.Game.Common;

public class BlackPlayerClient(string Username, string Uid) : FirebaseUserClient(Username, Uid)
{
    public BlackPlayerClient(FirebaseUser user) : this(user.DisplayName ?? "Black Player", user.Uid) { }
}