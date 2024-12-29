using System.Runtime.Serialization;

namespace Chess.ChessBoard;

/*[DataContract]*/
public class Queen(int id, (string, int) index, bool isWhite, ISpace space, Activity app) : BoardPiece(id, index, abbreviation, isWhite, space, app)
{
    private const char abbreviation = 'Q';

    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        this.Horizontals(board, pieces, ref moves);
        this.Verticals(board, pieces, ref moves);
        this.Diagonals(board, pieces, ref moves);
        return moves;
    }
}
