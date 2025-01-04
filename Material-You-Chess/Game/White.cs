using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;

namespace Chess.Game;

public class White : IPlayer
{
    /*[DataMember]*/
    public GameOutcome Outcome { get; set; } = GameOutcome.Ongoing;
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> Pieces { get; set; } = [];
    /*[DataMember]*/
    public Pawn[] Pawns { get; set; } = new Pawn[8];
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
    public White(Dictionary<(char, int), ISpace> Board, ConstraintLayout app)
    {
        char file = 'A';
        const int rank = 1;
        this.Rook1 = new Rook(Resource.Id.gmp__wRook1, ("wRook", 1), true, Board[(file, rank)], app);
        this.Pieces[("wRook", 1)] = this.Rook1;
        file++;//B

        this.Knight1 = new Knight(Resource.Id.gmp__wKnight1, ("wKnight", 1), true, Board[(file, rank)], app);
        this.Pieces[("wKnight", 1)] = this.Knight1;
        file++;//C

        this.Bishop1 = new Bishop(Resource.Id.gmp__wBishop1, ("wBishop", 1), true, Board[(file, rank)], app);
        this.Pieces[("wBishop", 1)] = this.Bishop1;
        file++;//D

        this.Queen = new Queen(Resource.Id.gmp__wQueen1, ("wQueen", 1), true, Board[(file, rank)], app);
        this.Pieces[("wQueen", 1)] = this.Queen;
        file++;//E

        this.King = new King(Resource.Id.gmp__wKing1, ("wKing", 1), true, Board[(file, rank)], app);
        this.Pieces[("wKing", 1)] = this.King;
        file++;//F

        this.Bishop2 = new Bishop(Resource.Id.gmp__wBishop2, ("wBishop", 2), true, Board[(file, rank)], app);
        this.Pieces[("wBishop", 2)] = this.Bishop2;
        file++;//G

        this.Knight2 = new Knight(Resource.Id.gmp__wKnight2, ("wKnight", 2), true, Board[(file, rank)], app);
        this.Pieces[("wKnight", 2)] = this.Knight2;
        file++;//H

        this.Rook2 = new Rook(Resource.Id.gmp__wRook2, ("wRook", 2), true, Board[(file, rank)], app);
        this.Pieces[("wRook", 2)] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++)
        {
            this.Pawns[i] = new Pawn(Resource.Id.gmp__wPawn1 + i, ("wPawn", i + 1), true, Board[(file, rank + 1)], app);
            this.Pieces[("wPawn", i + 1)] = this.Pawns[i];
            file++;
        }
    }

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