using Chess.Game.Board;

namespace Chess.Game.Moves;

public class PromoteRook(Pawn origin, ISpace destination) : IPromote
{
    public ISpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public Pawn Pawn { get; } = origin;

    public NetworkedPromoteRook ToNetworked() => new(this.Destination.Index, this.Origin.Index, this.OriginPiece.Index);
}


public class NetworkedPromoteRook((char, int) destination, (char, int) origin, (string, int) originPiece) : INetworkedPromote
{
    public (string, int) Pawn => originPiece;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;

#pragma warning disable CS9124
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
    public (string, int) OriginPiece { get; set; } = originPiece;
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
#pragma warning restore CS9124 

    public IMove FromNetworked(IChessGame game) => new PromoteRook((game.AllPieces[this.Pawn] as Pawn)!, game.Board[this.Destination]);
}