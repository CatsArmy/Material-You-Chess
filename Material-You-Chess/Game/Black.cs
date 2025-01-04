using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;

namespace Chess.Game;

public class Black : IPlayer
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

    public Black(Dictionary<(char, int), ISpace> Board, ConstraintLayout boardLayout)
    {
        char file = 'A';
        const int rank = 8;
        this.Rook1 = new Rook(Resource.Id.gmp__bRook1, ("bRook", 1), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bRook", 1)] = this.Rook1;
        file++;//B

        this.Knight1 = new Knight(Resource.Id.gmp__bKnight1, ("bKnight", 1), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bKnight", 1)] = this.Knight1;
        file++;//C

        this.Bishop1 = new Bishop(Resource.Id.gmp__bBishop1, ("bBishop", 1), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bBishop", 1)] = this.Bishop1;
        file++;//D

        this.Queen = new Queen(Resource.Id.gmp__bQueen1, ("bQueen", 1), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bQueen", 1)] = this.Queen;
        file++;//E

        this.King = new King(Resource.Id.gmp__bKing1, ("bKing", 1), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bKing", 1)] = this.King;
        file++;//F

        this.Bishop2 = new Bishop(Resource.Id.gmp__bBishop2, ("bBishop", 2), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bBishop", 2)] = this.Bishop2;
        file++;//G

        this.Knight2 = new Knight(Resource.Id.gmp__bKnight2, ("bKnight", 2), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bKnight", 2)] = this.Knight2;
        file++;//H

        this.Rook2 = new Rook(Resource.Id.gmp__bRook2, ("bRook", 2), false, Board[(file, rank)], boardLayout);
        this.Pieces[("bRook", 2)] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++, file++)
        {
            this.Pawns[i] = new Pawn(Resource.Id.gmp__bPawn1 + i, ("bPawn", i + 1), false, Board[(file, rank - 1)], boardLayout);
            this.Pieces[("bPawn", i + 1)] = this.Pawns[i];
        }
    }

    public bool IsInCheck(IPlayer enemy)
    {
        throw new NotImplementedException();
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