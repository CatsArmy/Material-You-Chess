using System.Collections.Generic;
using Android.Content.Res;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

public abstract class Piece : Space
{
    public ImageView piece;
    public int id;
    private static Resources res;

    public Piece(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(space, isWhite, spaceId)
    {
        this.piece = piece;
        this.id = id;
        space.Clickable = false;
    }

    public static void SetResources(Resources _res)
    {
        res = _res;
        Space.res = _res;
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
        this.piece.Clickable = true;
        this.space.Clickable = true;
        dest.space.Clickable = false;
        base.spaceId = dest.spaceId;
        base.space = dest.space;
        ConstraintLayout.LayoutParams @params = this.piece.LayoutParameters as ConstraintLayout.LayoutParams;
        @params.TopToTop = dest.spaceId;
        @params.BottomToBottom = dest.spaceId;
        @params.StartToStart = dest.spaceId;
        @params.EndToEnd = dest.spaceId;
        @params.VerticalBias = 0.5f;
        @params.HorizontalBias = 0.5f;

        @params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        @params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        @params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        @params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        this.piece.LayoutParameters = @params;

        return true;
    }


    public virtual bool Capture(Piece dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        return true;
    }

    //0 1 2 3 4 5 6
    //g m b _ _ A 1
    //|0|1||2|3|4|5|6|7|8|9|10|11|12|
    //|g|m||p|_|_|w|B|i|s|h|o |p |1 
    //|g|m||p|_|_|b|B|i|s|h|o |p |2 
    public (string, int) PieceKey()
    {
        Resources resources = res;
        string a = resources.GetResourceName(id);
        var spaceStr = string.Empty;
        for (int i = 5; i < a.Length - 1; i++)
        {
            spaceStr += a[i];
        }
        return (spaceStr, int.Parse($"{a[^1]}"));
    }
}
