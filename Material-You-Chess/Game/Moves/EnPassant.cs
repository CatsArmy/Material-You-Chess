using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class EnPassant : Capture
{
    internal EnPassant(Pawn origin, BoardSpace destination, Pawn Piece) : base(origin, destination, Piece) { }

    [method: JsonConstructor]
    public EnPassant(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : base(origin, destination, Piece) { }
}
