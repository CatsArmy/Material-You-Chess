using Chess.Game.Board;
using Newtonsoft.Json;

namespace Chess.Game.Moves;

public class EnPassant(IPiece origin, ISpace destination, Pawn captured) : Pawn.ISpecialMove, ICapture
{
    public ISpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public Pawn Pawn { get; } = captured;

    public IPiece Piece { get; } = captured;

    public NetworkedEnPassant ToNetworked() => new(this.OriginPiece.Index, this.Origin.Index, this.Pawn.Index, this.Destination.Index);

}

[JsonObject(MemberSerialization.OptOut)]
public class NetworkedEnPassant((string, int) originPiece, (char, int) origin, (string, int) captured, (char, int) destination) : Pawn.INetworkedSpecialMoves, INetworkedCapture
{
    public (string, int) Pawn => captured;
    public (string, int) Piece => captured;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;
    public (string, int) OriginPiece { get; set; } = originPiece;

    public IMove FromNetworked(IChessGame game) => new EnPassant(game.AllPieces[this.OriginPiece!], game.Board[this.Destination], (game.AllPieces[this.Pawn] as Pawn)!);
}
