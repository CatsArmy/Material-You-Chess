using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class Bishop(int id, (string, int) index, bool isWhite, ISpace space) : BoardPiece(id, index, _Abbreviation, isWhite, space)
{
    private const char _Abbreviation = 'B';
    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        this.Diagonals(board, pieces, ref moves);
        return moves;
    }
}
