using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class Knight(int id, (string, int) index, bool isWhite, ISpace space) : BoardPiece(id, index, _Abbreviation, isWhite, space)
{
    private const char _Abbreviation = 'N';
    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);

        var (upRight, upLeft) = DiagonalMovesUp(board);
        if (upRight != null)
        {
            if (upRight.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, upRight));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }
        if (upLeft != null)
        {
            if (upLeft.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, upLeft));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (downRight, downLeft) = DiagonalMovesDown(board);
        if (downRight != null)
        {
            if (downRight.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, downRight));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (downLeft != null)
        {
            if (downLeft.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, downLeft));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (rightUp, rightDown) = DiagonalMovesRight(board);
        if (rightUp != null)
        {
            if (rightUp.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, rightUp));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (rightDown != null)
        {
            if (rightDown.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, rightDown));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (leftUp, leftDown) = DiagonalMovesLeft(board);
        if (leftUp != null)
        {
            if (leftUp.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, leftUp));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (leftDown != null)
        {
            if (leftDown.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, leftDown));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }
}
