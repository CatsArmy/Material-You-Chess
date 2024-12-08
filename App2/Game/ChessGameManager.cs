//using System;
//using System.Collections.Generic;
//using Chess.ChessBoard;

//namespace Chess.Game;

//public interface ILocalGame : IChessGame
//{

//}

//public interface IOnlineGame : IChessGame
//{

//}

//public interface IChessGame
//{
//    public Dictionary<(string, int), Piece> AllPieces { get; set; }
//    public Dictionary<(string, int), Piece> WhitePieces { get; set; }
//    public Dictionary<(string, int), Piece> BlackPieces { get; set; }
//    public Dictionary<(char, int), Space> Board { get; set; }
//    public IPlayer Player1 { get; set; }
//    public IPlayer Player2 { get; set; }
//    public bool IsOngoing { get; set; }

//}


//[Serializable]
//public enum GameOutcome
//{
//    Draw,
//    Resign,
//    Win,
//    Lose,
//}


//public interface INetworkedPlayer : IPlayer
//{
//    public bool HasColorPreference { get; set; }
//}

//public interface IPlayer
//{
//    public bool IsWhite { get; set; }
//    public GameOutcome Outcome { get; set; }
//    internal Dictionary<(string, int), Piece> pieces { get; set; }
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