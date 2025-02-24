using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Bishop(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'B';

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Diagonals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
