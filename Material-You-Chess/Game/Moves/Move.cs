using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[JsonPolymorphic()]
[JsonDerivedType(typeof(Move), nameof(Move))]

[JsonDerivedType(typeof(Capture), nameof(Capture))]
[JsonDerivedType(typeof(MoveOnly), nameof(MoveOnly))]
[JsonDerivedType(typeof(EnPassant), nameof(EnPassant))]
[JsonDerivedType(typeof(Promotion), nameof(Promotion))]

[JsonDerivedType(typeof(DoubleMove), nameof(DoubleMove))]
[JsonDerivedType(typeof(KingSideCastle), nameof(KingSideCastle))]
[JsonDerivedType(typeof(QueenSideCastle), nameof(QueenSideCastle))]
[JsonDerivedType(typeof(PromotionCapture), nameof(PromotionCapture))]
public class Move(BoardPiece origin, BoardSpace destination) : IMove
{
    public BoardPiece Origin { get; set; } = origin;
    public BoardSpace Destination { get; set; } = destination;

    public virtual void IndicateMoveable()
    {
        this.Destination.IndicateMoveable();
        this.Origin.Space.IndicateMoveable();
    }

    public virtual void IndicateUnmovable()
    {
        this.Destination.IndicateUnmovable();
        this.Origin.Space.IndicateUnmovable();
        this.Origin.LastSpace?.IndicateUnmovable();
    }

    public virtual void Select()
    {
        this.Destination.Select();
        this.Origin.Space.Select();
    }

    public virtual void Unselect()
    {
        this.Destination.Unselect();
        this.Origin.Space.Unselect();
        this.Origin.LastSpace?.Unselect();
    }
}
