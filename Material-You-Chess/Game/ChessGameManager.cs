using System.Runtime.Serialization;
using Chess.ChessBoard;

namespace Chess.Game;


//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip
public interface IChessGame
{
    //[DataMember] public bool IsOngoing { get; set; }
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> AllPieces { get; set; }
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> WhitePieces { get; }
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> BlackPieces { get; }
    [IgnoreDataMember] public Dictionary<(char, int), ISpace> Board { get; }
    [DataMember] public IPlayer Player1 { get; set; }
    [DataMember] public IPlayer Player2 { get; set; }
}

public class ChessGame : IChessGame
{
    //[DataMember] public bool IsOngoing { get; set; }
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> AllPieces { get; set; } = new();
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> WhitePieces { get; } = new();
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> BlackPieces { get; } = new();
    [IgnoreDataMember] public Dictionary<(char, int), ISpace> Board { get; } = new();
    [DataMember] public IPlayer Player1 { get; set; }
    [DataMember] public IPlayer Player2 { get; set; }
}


[DataContract]
public enum GameOutcome
{
    [DataMember] Draw,
    [DataMember] Resign,
    [DataMember] Win,
    [DataMember] Lose,
}




public interface IPlayer
{
    [DataMember] public bool IsWhite { get; set; }
    [DataMember] public GameOutcome Outcome { get; set; }
    [IgnoreDataMember] public Dictionary<(string, int), IPiece> Pieces { get; set; }
}

//public interface INetworkedPlayer : IPlayer
//{
//    public bool HasColorPreference { get; set; }
//}
//public class ChessGameManager
//{
//    public ChessGameManager(IChessGame game)
//    {
//        this.Game = game;
//    }

//    public IChessGame Game;
//}

//public class LocalGame : ILocalGame
//{

//}

//public class OnlineGame : IOnlineGame
//{

//}