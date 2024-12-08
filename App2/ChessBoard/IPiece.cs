using System.Collections.Generic;

namespace Chess.ChessBoard;

public interface IPiece
{
    public void Move(Space dest);
    public abstract List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces);
}
