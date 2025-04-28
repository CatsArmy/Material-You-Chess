using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

public abstract class Castling(BoardPiece origin, BoardSpace destination) : MoveOnly(origin, destination)
{
    public abstract char File { get; }
    public abstract Rook? Rook { get; }
    public abstract MoveOnly PlayRook { get; }

    /// <summary> 
    /// When the move is played Shows a Visual Indicator of which spaces the king, rook safely passed through 
    /// </summary>
    public override void Select()
    {
        base.Select();
        this.PlayRook.Select();
    }

    /// <summary> 
    /// When the next move is played Hides the old Move Visual Indicator of which spaces the king, rook safely passed through 
    /// </summary>
    public override void Unselect()
    {
        base.Unselect();
        this.PlayRook.Unselect();
    }
}
