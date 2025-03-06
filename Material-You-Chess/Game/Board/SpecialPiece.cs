using Chess.Game.Moves;

namespace Chess.Game.Board;

public class SpecialPiece(int id, BoardSpace space) : BoardPiece(id, space)
{
    public bool HasMoved { get; set; } = false;

    public override void Move(Move move, ChessGame game)
    {
        this.Update();
        base.Move(move, game);
    }

    public override void Update(bool IsUpdatingPlayer = false)
    {
        base.Update(IsUpdatingPlayer);
        if (IsUpdatingPlayer)
            return;

        this.HasMoved = true;
    }
}
