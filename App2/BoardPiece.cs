using System.Collections.Generic;
using Android.Widget;

namespace Chess;

public abstract class BoardPiece : BoardSpace
{
    public ImageView piece;
    public int id;

    public BoardPiece(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(space, isWhite, spaceId)
    {
        this.piece = piece;
        this.id = id;
    }

    ///<remarks><code>
    ///
    ///var layout = piece.LayoutParameters as ConstraintLayout.LayoutParams;
    ///var top = Resources.GetResourceName(id);
    ///layout.StartToStart
    ///A : Start_start-Parent
    ///H : End_end-Parent
    ///1 : Bottom_bottom-Parent
    ///8 : Top_top-Parent
    ///
    /// </code></remarks>
    public abstract bool Move(BoardSpace dest, Dictionary<(string, int), BoardSpace> board);
}
