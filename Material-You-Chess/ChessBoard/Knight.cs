namespace Chess.ChessBoard;

[Serializable]
public class Knight(int id, bool isWhite, ISpace space) : BoardPiece(id, isWhite, space)
{
    public override List<Move> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<Move> moves = base.Moves(board, pieces);

        var (upRight, upLeft) = DiagonalMovesUp(board);
        if (upRight != null)
        {
            var p = upRight.Piece(pieces);
            if (p == null)
                moves.Add(new(upRight));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(upRight, true));
        }
        if (upLeft != null)
        {
            var p = upLeft.Piece(pieces);
            if (p == null)
                moves.Add(new(upLeft));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(upLeft, true));
        }

        var (downRight, downLeft) = DiagonalMovesDown(board);
        if (downRight != null)
        {
            var p = downRight.Piece(pieces);
            if (p == null)
                moves.Add(new(downRight));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(downRight, true));
        }
        if (downLeft != null)
        {
            var p = downLeft.Piece(pieces);
            if (p == null)
                moves.Add(new(downLeft));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(downLeft, true));
        }

        var (rightUp, rightDown) = DiagonalMovesRight(board);
        if (rightUp != null)
        {
            var p = rightUp.Piece(pieces);
            if (p == null)
                moves.Add(new(rightUp));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(rightUp, true));
        }
        if (rightDown != null)
        {
            var p = rightDown.Piece(pieces);
            if (p == null)
                moves.Add(new(rightDown));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(rightDown, true));
        }

        var (leftUp, leftDown) = DiagonalMovesLeft(board);
        if (leftUp != null)
        {
            var p = leftUp.Piece(pieces);
            if (p == null)
                moves.Add(new(leftUp));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(leftUp, true));
        }
        if (leftDown != null)
        {
            var p = leftDown.Piece(pieces);
            if (p == null)
                moves.Add(new(leftDown));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(leftDown, true));
        }

        return moves;
    }
}
