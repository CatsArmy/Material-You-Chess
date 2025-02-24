using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Queen(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'Q';

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game.Board, game.AllPieces, ref moves);
        this.Verticals(game.Board, game.AllPieces, ref moves);
        this.Diagonals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
