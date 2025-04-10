using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public class EnPassant(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : Capture(origin, destination, Piece)
{
    public EnPassant(Pawn origin, BoardSpace destination, Pawn Piece) : this(origin as BoardPiece, destination, Piece) { }
}
