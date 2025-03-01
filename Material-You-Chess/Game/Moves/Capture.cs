using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Capture : Move
{
    public BoardPiece Piece { get; set; }

    public Capture(BoardPiece origin, BoardPiece destination) : base(origin, destination.Space)
    {
        this.Piece = destination;
    }

    [JsonConstructor]
    public Capture(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : base(origin, destination)
    {
        this.Piece = Piece;
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
