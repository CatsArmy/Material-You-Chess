using Chess.Game.Board;

namespace Chess.Game.Moves;

public class QueenSideCastle(Rook rook, King king) : MoveOnly(king, rook.Space)
{
    public King King { get; } = king;
    public Rook Rook { get; } = rook;

    public override void IndicateMoveable()
    {

    }

    public override void IndicateUnmovable()
    {

    }

    public override void Select()
    {

    }

    public override void Unselect()
    {

    }
}
