using System.Text.Json.Serialization;
using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

[method: JsonConstructor]
public class DoubleMove(BoardPiece origin, BoardSpace destination) : MoveOnly(origin, destination)
{
    public DoubleMove(Pawn origin, BoardSpace destination) : this(origin as BoardPiece, destination) { }
}
