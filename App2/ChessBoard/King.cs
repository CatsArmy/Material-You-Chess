using System.Collections.Generic;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

public class King : Piece
{
    public King(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        if (true)
        {
            base.piece.LayoutParameters = dest.space.LayoutParameters as ConstraintLayout.LayoutParams;
            base.spaceId = dest.spaceId;
            return true;
            return base.Move(dest, board, pieces);
        }
    }
}
