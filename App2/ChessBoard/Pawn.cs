using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

public class Pawn : Piece
{
    private bool isFirstMove = true;

    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;
        if (!isLegalMove)
            return false;
        //promote logic
        this.isFirstMove = false;
        return base.Move(dest, board, pieces);
    }

    public override List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        Space move = this.Forward(board, this.isWhite);
        var piece = move.GetPiece(pieces);
        if (piece == null)
        {
            moves.Add(new(move));
            if (this.isFirstMove)
            {
                var move2 = move.Forward(board, this.isWhite);
                if (move2.GetPiece(pieces) == null)
                    moves.Add(new(move2, false, true));
            }
        }

        (Space left, Space right) = this.isWhite switch
        {
            true => (DiagonalUp(board, false), DiagonalUp(board, true)),
            false => (DiagonalDown(board, false), DiagonalDown(board, true)),
        };

        if (left != null && left.GetPiece(pieces) != null)
            if (left.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(left, true));

        if (right != null && right.GetPiece(pieces) != null)
            if (right.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(right, true));

        return moves;
    }

    public void Promote()
    {
        //Get name via id
        //create a (string,int) key with the name
        //remove the value from the dict with the key
        //Open Promote dialog/popup thingy
        //add back the value but as a the selected piece(cant be king)
    }
}
