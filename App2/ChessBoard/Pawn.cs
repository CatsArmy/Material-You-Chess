using System.Collections.Generic;
using System.Linq;
using Android.Widget;

namespace Chess.ChessBoard;

public class Pawn : Piece
{
    private bool isFirstMove = true;

    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;

        var moves = GetMoves(board, pieces);
        var move = moves.FirstOrDefault(i => i.Space.spaceId == dest.spaceId);
        if (move == null)
            return false;
        this.isFirstMove = false;
        if (!isLegalMove)
            return false;
        //promote logic
        return base.Move(dest, board, pieces);
    }

    public List<Move> GetMoves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        //move is null???
        Space move = this.Forward(board);
        var c = move.GetPiece(pieces);
        if (c == null)
        {
            moves.Add(new(move));
            if (this.isFirstMove)
            {
                var a = c.Forward(board);
                var b = a.GetPiece(pieces);
                if (b == null)
                    moves.Add(new(a, false, true));
            }
        }

        (Space left, Space right) = this.isWhite switch
        {
            true => (DiagonalUp(board, false), DiagonalUp(board, true)),
            false => (DiagonalDown(board, false), DiagonalDown(board, true)),
        };

        if (left.GetPiece(pieces) != null)
        {
            if (left.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(left, true));
        }

        if (right.GetPiece(pieces) != null)
        {
            if (right.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(right, true));
        }
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
