using Chess.Game.Board;

namespace Chess.Game.Moves;

public abstract class Castling(BoardPiece origin, BoardSpace destination) : MoveOnly(origin, destination)
{
    public abstract char File { get; }
    public abstract Rook? Rook { get; }
    public abstract MoveOnly PlayRook { get; }
}
