using Chess.ChessBoard;

namespace Chess.Game;

public interface IPlayer
{
    /*[DataMember]*/
    public GameOutcome Outcome { get; set; }
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> Pieces { get; set; }
    /*[DataMember]*/
    public Pawn[] Pawns { get; set; }
    /*[DataMember]*/
    public Rook? Rook1 { get; set; }
    /*[DataMember]*/
    public Knight? Knight1 { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop1 { get; set; }
    /*[DataMember]*/
    public King? King { get; set; }
    /*[DataMember]*/
    public Queen? Queen { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop2 { get; set; }
    /*[DataMember]*/
    public Knight? Knight2 { get; set; }
    /*[DataMember]*/
    public Rook? Rook2 { get; set; }
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