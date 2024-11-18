using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

public abstract class Piece : Space
{
    public ImageView piece;
    public int id;

    public Piece(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(space, isWhite, spaceId)
    {
        this.piece = piece;
        this.id = id;
        space.Clickable = false;
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
    public virtual bool Move(Space dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        space.Clickable = true;
        dest.space.Clickable = false;
        return true;
    }

    public virtual bool Capture(Piece dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        return true;
    }
}
