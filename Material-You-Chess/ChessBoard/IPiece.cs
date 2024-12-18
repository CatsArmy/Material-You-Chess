namespace Chess.ChessBoard;

public interface IPiece
{
    public void Move(BoardSpace dest);
    public abstract List<Move> Moves(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces);
}
