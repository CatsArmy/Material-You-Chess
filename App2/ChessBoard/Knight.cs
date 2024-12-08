using System;
using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

[Serializable]
public class Knight : Piece
{
    public Knight(ImageView piece, int id, ImageView space, bool isWhite, int spaceId, Action callback) : base(piece, id, space, isWhite, spaceId, callback) { }

    public override List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();

        var (upRight, upLeft) = MovesUp(board);
        if (upRight != null)
        {
            var p = upRight.GetPiece(pieces);
            if (p == null)
                moves.Add(new(upRight));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(upRight, true));
        }
        if (upLeft != null)
        {
            var p = upLeft.GetPiece(pieces);
            if (p == null)
                moves.Add(new(upLeft));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(upLeft, true));
        }

        var (downRight, downLeft) = MovesDown(board);
        if (downRight != null)
        {
            var p = downRight.GetPiece(pieces);
            if (p == null)
                moves.Add(new(downRight));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(downRight, true));
        }
        if (downLeft != null)
        {
            var p = downLeft.GetPiece(pieces);
            if (p == null)
                moves.Add(new(downLeft));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(downLeft, true));
        }

        var (rightUp, rightDown) = MovesRight(board);
        if (rightUp != null)
        {
            var p = rightUp.GetPiece(pieces);
            if (p == null)
                moves.Add(new(rightUp));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(rightUp, true));
        }
        if (rightDown != null)
        {
            var p = rightDown.GetPiece(pieces);
            if (p == null)
                moves.Add(new(rightDown));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(rightDown, true));
        }

        var (leftUp, leftDown) = MovesLeft(board);
        if (leftUp != null)
        {
            var p = leftUp.GetPiece(pieces);
            if (p == null)
                moves.Add(new(leftUp));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(leftUp, true));
        }
        if (leftDown != null)
        {
            var p = leftDown.GetPiece(pieces);
            if (p == null)
                moves.Add(new(leftDown));
            else if (p.isWhite != this.isWhite)
                moves.Add(new(leftDown, true));
        }

        return moves;
    }

    private (Space, Space) MovesUp(Dictionary<(char, int), Space> board) => this.KnightMovesUp(board);
    private (Space, Space) MovesDown(Dictionary<(char, int), Space> board) => this.KnightMovesDown(board);
    private (Space, Space) MovesRight(Dictionary<(char, int), Space> board) => this.KnightMovesRight(board);
    private (Space, Space) MovesLeft(Dictionary<(char, int), Space> board) => this.KnightMovesLeft(board);
}
