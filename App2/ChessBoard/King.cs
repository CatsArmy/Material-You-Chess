using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;

namespace Chess.ChessBoard;

[Serializable]
public class King : Piece
{
    public bool HasMoved = false;
    public King(ImageView piece, int id, ImageView space, bool isWhite, int spaceId, Action callback) : base(piece, id, space, isWhite, spaceId, callback) { }

    public override List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();

        Space up = this.Up(board);
        if (up != null)
            if (up.GetPiece(pieces) == null)
                moves.Add(new(up));
            else if (up.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(up, true));

        Space down = this.Down(board);
        if (down != null)
            if (down.GetPiece(pieces) == null)
                moves.Add(new(down));
            else if (down.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(down, true));
        Space left = this.Left(board);
        if (left != null)
            if (left.GetPiece(pieces) == null)
                moves.Add(new(left));
            else if (left.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(left, true));

        Space right = this.Right(board);
        if (right != null)
            if (right.GetPiece(pieces) == null)
                moves.Add(new(right));
            else if (right.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(right, true));

        Space topLeft = DiagonalUp(board, false);
        if (topLeft != null)
            if (topLeft.GetPiece(pieces) == null)
                moves.Add(new(topLeft));
            else if (topLeft.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(topLeft, true));

        Space topRight = DiagonalUp(board, true);
        if (topRight != null)
            if (topRight.GetPiece(pieces) == null)
                moves.Add(new(topRight));
            else if (topRight.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(topRight, true));

        Space bottomLeft = DiagonalDown(board, false);
        if (bottomLeft != null)
            if (bottomLeft.GetPiece(pieces) == null)
                moves.Add(new(bottomLeft));
            else if (bottomLeft.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(bottomLeft, true));

        Space bottomRight = DiagonalDown(board, true);
        if (bottomRight != null)
            if (bottomRight.GetPiece(pieces) == null)
                moves.Add(new(bottomRight));
            else if (bottomRight.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(bottomRight, true));

        return moves;
    }

    public bool IsInCheck(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        char key = this.isWhite ? 'b' : 'w';
        List<Move> moves = new List<Move>();
        foreach (var piece in pieces)
        {
            if (!piece.Key.Item1.StartsWith(key))
                continue;

            moves.AddRange(piece.Value.Moves(board, pieces));
        }
        moves = moves.Where(move => move.Capture).ToList();
        return moves.FirstOrDefault(move => IsCheck(move, pieces)) != null;
    }

    public bool IsCheck(Move move, Dictionary<(string, int), Piece> pieces)
    {
        var piece = move.Space.GetPiece(pieces);
        if (piece == null)
            return false;
        return piece == this;
    }
}