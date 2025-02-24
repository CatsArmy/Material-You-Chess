using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Knight(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'N';
    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);

        var (upRight, upLeft) = DiagonalMovesUp(game.Board);
        if (upRight != null)
        {
            if (upRight.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, upRight));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }
        if (upLeft != null)
        {
            if (upLeft.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, upLeft));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (downRight, downLeft) = DiagonalMovesDown(game.Board);
        if (downRight != null)
        {
            if (downRight.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, downRight));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (downLeft != null)
        {
            if (downLeft.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, downLeft));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (rightUp, rightDown) = DiagonalMovesRight(game.Board);
        if (rightUp != null)
        {
            if (rightUp.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, rightUp));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (rightDown != null)
        {
            if (rightDown.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, rightDown));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var (leftUp, leftDown) = DiagonalMovesLeft(game.Board);
        if (leftUp != null)
        {
            if (leftUp.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, leftUp));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (leftDown != null)
        {
            if (leftDown.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, leftDown));
            else if (piece.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }
}
