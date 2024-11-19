using System.Collections.Generic;
using Android.Content.Res;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

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
        piece.Clickable = true;
        space.Clickable = true;
        dest.space.Clickable = false;
        piece.LayoutParameters = dest.space.LayoutParameters as ConstraintLayout.LayoutParams;
        base.spaceId = dest.spaceId;
        return true;
    }

    public virtual bool Capture(Piece dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        return true;
    }

    public (string, int) PieceKey(Resources res)
    {
        string a = res.GetResourceName(id);
        //0 1 2 3 4 5 6
        //g m b _ _ A 1
        //|0|1||2|3|4|5|6|7|8|9|10|11|12|
        //|g|m||p|_|_|w|B|i|s|h|o |p |1 
        //|g|m||p|_|_|b|B|i|s|h|o |p |2 
        var spaceStr = string.Empty;
        for (int i = 5; i < a.Length - 1; i++)
        {
            spaceStr += a[i];
        }
        return (spaceStr, int.Parse($"{a[^1]}"));
    }
}
