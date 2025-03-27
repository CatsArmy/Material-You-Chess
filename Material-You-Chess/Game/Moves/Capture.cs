using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public class Capture(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : Move(origin, destination)
{
    public Capture(BoardPiece origin, BoardPiece destination) : this(origin, destination.Space, destination) { }

    public BoardPiece Piece { get; set; } = Piece;

    public override void IndicateMoveable()
    {
        base.IndicateMoveable();
        this.Piece.Space.IndicateMoveable();
    }

    public override void UnindicateMoveable()
    {
        base.UnindicateMoveable();
        this.Piece.Space.IndicateUnmovable();
    }

    public override void Select()
    {
        base.Select();
        this.Piece.Space.Select();
    }

    public override void Unselect()
    {
        base.Unselect();
        this.Piece.Space.Unselect();
    }
}
