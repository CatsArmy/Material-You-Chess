using Chess.Game.Board;

namespace Chess.Game.Moves;

public class EnPassant(BoardPiece origin, BoardSpace destination, Pawn captured) : Move(origin, destination)
{
    public Pawn Pawn { get; } = captured;
}
