using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class DoubleMove : MoveOnly
{
    public DoubleMove(Pawn origin, BoardSpace destination) : base(origin, destination) { }

    [JsonConstructor]
    public DoubleMove(BoardPiece origin, BoardSpace destination) : base(origin, destination) { }
}
