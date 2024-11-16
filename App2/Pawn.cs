using System.Collections.Generic;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess;

public class Pawn : BoardPiece
{
    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(BoardSpace dest, Dictionary<(string, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces)
    {
        base.piece.LayoutParameters = dest.space.LayoutParameters as ConstraintLayout.LayoutParams;
        base.spaceId = dest.spaceId;
        return true;
    }
}
