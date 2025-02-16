using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Capture(IPiece origin, IPiece destination) : ICapture
{
    public string Type { get; } = nameof(Capture);
    public ISpace Destination { get; set; } = destination.Space;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public IPiece Piece { get; } = destination;

    public NetworkedCapture ToNetworked() => new(Piece.Index, Destination.Index, Origin.Index, OriginPiece.Index);
}

public class NetworkedCapture((string, int) piece, (char, int) destination, (char, int) origin, (string, int) originPiece) : INetworkedCapture
{
    public (string, int) Piece { get; } = piece;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;
    public (string, int) OriginPiece { get; set; } = originPiece;

    public IMove FromNetworked(IChessGame game) => new Capture(game.AllPieces[OriginPiece], game.AllPieces[Piece]);
}


public interface ICastle : IMove
{
    public King King { get; }
    public Rook Rook { get; }

    public new void Select()
    {
        this.Destination.SelectSpace();
        this.Origin.SelectSpace();
        this.King.Space.SelectSpace();
        this.Rook.Space.SelectSpace();
    }

    public new void Unselect()
    {
        this.Destination.UnselectSpace();
        this.Origin.UnselectSpace();
        this.King.Space.SelectSpace();
        this.Rook.Space.SelectSpace();
    }
}

public interface INetworkedCastle : INetworkedMove
{
    public (string, int) Pawn { get; }
}

public class KingSideCastle(Rook rook, King king)// : ICastle
{
    public string Type { get; } = nameof(KingSideCastle);
    public ISpace Destination { get; set; } = king.Space;

    public int DestinationId { get; set; } = king.Id;

    public ISpace Origin { get; set; } = rook.Space;

    public IPiece OriginPiece { get; set; } = rook;

    public int OriginId { get; set; } = rook.Id;

    public King King { get; } = king;
    public Rook Rook { get; } = rook;


    //public NetworkedKingSideCastle ToNetworked() => new(this.Destination.Index, this.Origin.Index, this.OriginPiece.Index);
}
public class QueenSideCastle(Rook rook, King king)// : ICastle
{
    public string Type { get; } = nameof(KingSideCastle);
    public ISpace Destination { get; set; } = king.Space;

    public int DestinationId { get; set; } = king.Id;

    public ISpace Origin { get; set; } = rook.Space;

    public IPiece OriginPiece { get; set; } = rook;

    public int OriginId { get; set; } = rook.Id;

    public King King { get; } = king;
    public Rook Rook { get; } = rook;


    //public NetworkedKingSideCastle ToNetworked() => new(this.Destination.Index, this.Origin.Index, this.OriginPiece.Index);
}


//public class NetworkedKingSideCastle((char, int) destination, (char, int) origin, (string, int) originPiece) : INetworkedCastle
//{
//    public (string, int) Pawn => originPiece;
//    public (char, int) Destination { get; set; } = destination;
//    public (char, int) Origin { get; set; } = origin;

//#pragma warning disable CS9124
//    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
//    public (string, int) OriginPiece { get; set; } = originPiece;
//    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
//#pragma warning restore CS9124

//    public IMove FromNetworked(IChessGame game) => new KingSideCastle((game.AllPieces[this.King] as )!, game.Board[this.Destination]);
//}