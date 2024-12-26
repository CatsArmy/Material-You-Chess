namespace Chess.ChessBoard;

public class Move(IPiece origin, ISpace destination) : IMove
{
    public ISpace Destination { get; set; } = destination;
    public int DestinationId { get; set; } = destination.Id;
    public ISpace Origin { get; set; } = origin.Space;
    public IPiece OriginPiece { get; set; } = origin;
    public int OriginId { get; set; } = origin.Id;
}

public interface IMove
{
    public ISpace Destination { get; set; }
    public int DestinationId { get; set; }
    public ISpace Origin { get; set; }
    public IPiece OriginPiece { get; set; }
    public int OriginId { get; set; }
}

public class EnPassantMove(IPiece origin, ISpace destination, Pawn captured) : IEnPassant
{
    public ISpace Destination { get; set; } = destination;
    public int DestinationId { get; set; } = destination.Id;
    public ISpace Origin { get; set; } = origin.Space;
    public IPiece OriginPiece { get; set; } = origin;
    public int OriginId { get; set; } = origin.Id;
    public Pawn Pawn { get; } = captured;
    public IPiece Piece { get; } = captured;
}
public class Capture(IPiece origin, IPiece destination) : ICapture
{
    public ISpace Destination { get; set; } = destination.Space;
    public int DestinationId { get; set; } = destination.Id;
    public ISpace Origin { get; set; } = origin.Space;
    public IPiece OriginPiece { get; set; } = origin;
    public int OriginId { get; set; } = origin.Id;
    public IPiece Piece { get; } = destination;
}

public interface IEnPassant : ICapture
{
    public Pawn Pawn { get; }
}

public interface ICapture : IMove
{
    public IPiece Piece { get; }
}