using System;
using System.Collections.Generic;
using Chess.ChessBoard;

//namespace Chess.Game;

//public interface ILocalGame : IChessGame
//{

//}

//public interface IOnlineGame : IChessGame
//{

//}


//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip
public interface IChessGame
{
    public Dictionary<(string, int), IPiece> AllPieces { get; set; }
    public Dictionary<(string, int), IPiece> WhitePieces { get; set; }
    public Dictionary<(string, int), IPiece> BlackPieces { get; set; }
    public Dictionary<(char?, int?), BoardSpace> Board { get; set; }
    public IPlayer Player1 { get; set; }
    public IPlayer Player2 { get; set; }
    public bool IsOngoing { get; set; }
    //public bool IsOngoing { get; set; }
}


[Serializable]
public enum GameOutcome
{
    Draw,
    Resign,
    Win,
    Lose,
}


//public interface INetworkedPlayer : IPlayer
//{
//    public bool HasColorPreference { get; set; }
//}

public interface IPlayer
{
    public bool IsWhite { get; set; }
    public GameOutcome Outcome { get; set; }
    internal Dictionary<(string, int), IPiece> pieces { get; set; }

}

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