using Android.Animation;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class SpecialPiece(int id, BoardSpace space) : BoardPiece(id, space)
{
    public bool HasMoved { get; set; } = false;

    public override void Move(Move move, ChessGame game)
    {
        if (game.Player == null || game.Enemy == null)
            return;

        game.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
        game.LastMove = move;

        if (move is Capture capture)
        {
            this.Capture(capture.Piece, game);
        }
    }

    public override void Update(bool IsUpdatingPlayer = false)
    {
        base.Update(IsUpdatingPlayer);
        if (IsUpdatingPlayer)
            return;

        this.HasMoved = true;
    }
}