using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
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
                moves.Add(new(this.Space, upRight));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, upRight, true));
        }
        if (upLeft != null)
        {
            var p = upLeft.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, upLeft));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, upLeft, true));
        }

        var (downRight, downLeft) = DiagonalMovesDown(board);
        if (downRight != null)
        {
            var p = downRight.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, downRight));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, downRight, true));
        }
        if (downLeft != null)
        {
            var p = downLeft.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, downLeft));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, downLeft, true));
        }

        var (rightUp, rightDown) = DiagonalMovesRight(board);
        if (rightUp != null)
        {
            var p = rightUp.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, rightUp));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, rightUp, true));
        }
        if (rightDown != null)
        {
            var p = rightDown.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, rightDown));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, rightDown, true));
        }

        var (leftUp, leftDown) = DiagonalMovesLeft(board);
        if (leftUp != null)
        {
            var p = leftUp.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, leftUp));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, leftUp, true));
        }
        if (leftDown != null)
        {
            var p = leftDown.Piece(pieces);
            if (p == null)
                moves.Add(new(this.Space, leftDown));
            else if (p.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, leftDown, true));
        }

        return moves;
    }
}
