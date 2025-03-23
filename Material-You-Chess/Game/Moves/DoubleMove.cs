using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public class DoubleMove(BoardPiece origin, BoardSpace destination) : MoveOnly(origin, destination)
{
    public DoubleMove(Pawn origin, BoardSpace destination) : this(origin as BoardPiece, destination) { }
}
