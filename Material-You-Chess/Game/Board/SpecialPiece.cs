namespace Chess.Game.Board;

public class SpecialPiece(int id, BoardSpace space) : BoardPiece(id, space)
{
    public bool HasMoved { get; set; } = false;

    public virtual void Update() => this.HasMoved = true;
}
