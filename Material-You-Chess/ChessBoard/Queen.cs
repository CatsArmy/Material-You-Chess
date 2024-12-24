using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class Queen(int id, bool isWhite, ISpace space) : BoardPiece(id, isWhite, space)
{
    public override List<Move> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<Move> moves = base.Moves(board, pieces);
        this.Horizontals(board, pieces, ref moves);
        this.Verticals(board, pieces, ref moves);
        this.Diagonals(board, pieces, ref moves);
        return moves;
    }
}
