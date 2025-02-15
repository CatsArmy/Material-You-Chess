namespace Chess.Game.Moves;

public class Move(IPiece origin, ISpace destination) : IMove
{
    public ISpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public NetworkedMove ToNetworked() => new(Destination.Index, Origin.Index, OriginPiece.Index);
}


public class NetworkedMove((char, int) destination, (char, int) origin, (string, int) originPiece) : INetworkedMove
{
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;
    public (string, int) OriginPiece { get; set; } = originPiece;

    public IMove FromNetworked(IChessGame game) => new Move(game.AllPieces[OriginPiece], game.Board[Destination]);
}