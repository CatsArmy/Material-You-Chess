using Android.Widget;

namespace Chess;

public class BoardSpace
{
    public ImageView space;
    public bool isWhite;
    public int spaceId;

    public BoardSpace(ImageView space, bool isWhite, int spaceId)
    {
        this.space = space;
        this.isWhite = isWhite;
        this.spaceId = spaceId;
    }
}
