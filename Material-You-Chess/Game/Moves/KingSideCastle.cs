using Chess.Game.Board;

namespace Chess.Game.Moves;

public class KingSideCastle(Rook rook, King king) : MoveOnly(king, rook.Space)
{
    public King King { get; } = king;
    public Rook Rook { get; } = rook;

    public override void Select()
    {
        base.Select();
        this.King.Space.Select();
        this.Rook.Space.Select();
    }

    public override void Unselect()
    {
        base.Unselect();
        this.King.Space.Select();
        this.Rook.Space.Select();
    }
}
