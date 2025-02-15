using Chess.Game.Board;

namespace Chess.Game.Moves;

public class PromoteRookAndCapture(Pawn origin, IPiece destination) : IPromoteAndCapture
{
    public ISpace Destination { get; set; } = destination.Space;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public Pawn Pawn { get; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public IPiece Piece { get; } = destination;

    public NetworkedPromoteRookAndCapture ToNetworked() => new(Piece.Index, Destination.Index, Origin.Index, OriginPiece.Index);
}


public class NetworkedPromoteRookAndCapture((string, int) piece, (char, int) destination, (char, int) origin, (string, int) originPiece)
    : INetworkedPromoteAndCapture
{
    public (string, int) Piece { get; } = piece;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;
#pragma warning disable CS9124
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
    public (string, int) OriginPiece { get; set; } = originPiece;
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
#pragma warning restore CS9124 
    public (string, int) Pawn => originPiece;

    public IMove FromNetworked(IChessGame game) => new PromoteRookAndCapture((game.AllPieces[OriginPiece] as Pawn)!, game.AllPieces[Piece]);
}
