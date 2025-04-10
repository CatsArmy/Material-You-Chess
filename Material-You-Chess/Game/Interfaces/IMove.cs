using Chess.Game.Board;

namespace Chess.Game.Interfaces;

public interface IMove
{
    public BoardPiece Origin { get; set; }

    public BoardSpace Destination { get; set; }

    public void IndicateMoveable();

    public void UnindicateMoveable();
}