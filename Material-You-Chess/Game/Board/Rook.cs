using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Rook(int id, (string, int) index, bool isWhite, ISpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout), ISpecialBoardPiece
{
    public bool HasMoved { get; set; } = false;

    private const char abbreviation = 'R';
    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        this.Horizontals(board, pieces, ref moves);
        this.Verticals(board, pieces, ref moves);
        return moves;
    }
}
