using Android.Content.Res;
using Android.Widget;

namespace Chess.ChessBoard;

public class Space
{
    public ImageView space;
    public bool isWhite;
    public int spaceId;
    internal static Resources res;


    public Space(ImageView space, bool isWhite, int spaceId)
    {
        this.space = space;
        this.isWhite = isWhite;
        this.spaceId = spaceId;
    }

    public (string, int) GetSpaceKey()
    {
        string a = res.GetResourceName(this.spaceId);
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
        return ($"{a[^2]}", int.Parse($"{a[^1]}"));

    }
}
