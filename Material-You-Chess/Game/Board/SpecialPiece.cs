using Chess.Game.Moves;

namespace Chess.Game.Board;

public class SpecialPiece(int id, BoardSpace space) : BoardPiece(id, space)
{
    public bool HasMoved { get; set; } = false;

    public override void Update()
    {
        base.Update();
        this.HasMoved = true;
    }
}
