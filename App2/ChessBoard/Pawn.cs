using System.Collections.Generic;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

public class Pawn : Piece
{
    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        base.piece.LayoutParameters = dest.space.LayoutParameters as ConstraintLayout.LayoutParams;
        base.spaceId = dest.spaceId;
        //promote logic
        return true;
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
