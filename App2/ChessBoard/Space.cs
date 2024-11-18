using Android.Widget;

namespace Chess.ChessBoard;

public class Space
{
    public ImageView space;
    public bool isWhite;
    public int spaceId;

    public Space(ImageView space, bool isWhite, int spaceId)
    {
        this.space = space;
        this.isWhite = isWhite;
        this.spaceId = spaceId;
    }
}
