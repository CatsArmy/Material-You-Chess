using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Rook(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : SpecialPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'R';

    public override void Move(Move move, ChessGame game)
    {
        base.Move(move, game);
        this.Move(move);
        game.NextTurn(move);
    }

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game.Board, game.AllPieces, ref moves);
        this.Verticals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
