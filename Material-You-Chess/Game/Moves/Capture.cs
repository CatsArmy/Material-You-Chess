namespace Chess.Game.Moves;

public class Capture(IPiece origin, IPiece destination) : ICapture
{
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