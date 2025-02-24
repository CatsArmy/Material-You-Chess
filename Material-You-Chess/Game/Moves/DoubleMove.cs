using Chess.Game.Board;

namespace Chess.Game.Moves;

public class DoubleMove(Pawn origin, BoardSpace destination) : MoveOnly(origin, destination);
