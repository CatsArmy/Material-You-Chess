using Android.Animation;
using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class SpecialPiece(int id, (string, int) index, char abbreviation, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    public bool HasMoved { get; set; } = false;

    public override void Move(Move move, ChessGame game)
    {
        if (game.Player == null || game.Enemy == null)
            return;

        game.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
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