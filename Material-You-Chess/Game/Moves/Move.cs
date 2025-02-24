using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[JsonPolymorphic()]
[JsonDerivedType(typeof(Move), nameof(Move))]

[JsonDerivedType(typeof(Capture), nameof(Capture))]
[JsonDerivedType(typeof(MoveOnly), nameof(MoveOnly))]
[JsonDerivedType(typeof(EnPassant), nameof(EnPassant))]
[JsonDerivedType(typeof(Promotion), nameof(Promotion))]

[JsonDerivedType(typeof(KingSideCastle), nameof(KingSideCastle))]
[JsonDerivedType(typeof(QueenSideCastle), nameof(QueenSideCastle))]
[JsonDerivedType(typeof(DoubleMove), nameof(DoubleMove))]
[JsonDerivedType(typeof(PromotionCapture), nameof(PromotionCapture))]
public class Move(BoardPiece origin, BoardSpace destination) : IMove
{
    public BoardSpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public BoardSpace Origin { get; set; } = origin.Space;

    public BoardPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public virtual void Select()
    {
        this.Destination.Select();
        this.Origin.Select();
    }

    public virtual void Unselect()
    {
        this.Destination.Unselect();
        this.Origin.Unselect();
    }
}
