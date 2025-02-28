using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class DoubleMove : MoveOnly
{
    [JsonConstructor]
    public DoubleMove(Pawn origin, BoardSpace destination) : base(origin, destination) { }
}
