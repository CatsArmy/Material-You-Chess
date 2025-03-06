using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Castling(BoardPiece origin, BoardSpace destination) : MoveOnly(origin, destination)
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public virtual MoveOnly Rook { get; }
    public virtual MoveOnly King { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public override void IndicateMoveable()
    {
        this.Rook?.IndicateMoveable();
        this.King?.IndicateMoveable();
    }

    public override void IndicateUnmovable()
    {
        this.Rook?.IndicateUnmovable();
        this.King?.IndicateUnmovable();
    }

    public override void Select()
    {
        this.Rook?.Select();
        this.King?.Select();
    }

    public override void Unselect()
    {
        this.Rook?.Unselect();
        this.King?.Unselect();
    }
}
