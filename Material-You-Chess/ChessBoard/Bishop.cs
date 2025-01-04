using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

/*[DataContract]*/
public class Bishop(int id, (string, int) index, bool isWhite, ISpace space, ConstraintLayout boardLayout) : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'B';

    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        this.Diagonals(board, pieces, ref moves);
        return moves;
    }
}
