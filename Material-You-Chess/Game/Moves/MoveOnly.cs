using Chess.Game.Board;

namespace Chess.Game.Moves;

public class MoveOnly(BoardPiece origin, BoardSpace destination) : Move(origin, destination);
