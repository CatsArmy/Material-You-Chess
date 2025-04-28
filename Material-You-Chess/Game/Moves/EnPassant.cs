using System.Text.Json.Serialization;
using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

[method: JsonConstructor]
public class EnPassant(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : Capture(origin, destination, Piece)
{
    public EnPassant(Pawn origin, BoardSpace destination, Pawn Piece) : this(origin as BoardPiece, destination, Piece) { }
}
