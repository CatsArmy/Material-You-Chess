using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Capture(BoardPiece origin, BoardPiece destination) : Move(origin, destination.Space)
{
    public BoardPiece Piece { get; internal set; } = destination;
}
