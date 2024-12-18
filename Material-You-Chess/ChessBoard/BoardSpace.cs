using Android.Content.Res;
using Android.Util;

namespace Chess.ChessBoard;

[Serializable]
public class BoardSpace
{
    [NonSerialized]
    protected static Resources res;

    public BoardSpace(Resources _res) => res = _res;

    [NonSerialized]
    public ImageView space;
    public bool isWhite;
    public int spaceId;

    public BoardSpace(ImageView space, bool isWhite, int spaceId, Resources resources = null)
    {
        this.space = space;
        this.isWhite = isWhite;
        this.spaceId = spaceId;
        if (res == null && resources != null)
            res = resources;
    }

    public void SelectSpace()
    {
        //var colorId = isWhite ? Resource.Color.white_space_overlay_color : Resource.Color.black_space_overlay_color;
        /////<returns> </returns>A single color value in the form 0xAARRGGBB.
        //var colorARGB = ContextCompat.GetColor(context, colorId);

        space.SetImageLevel(1);
    }

    public void UnselectSpace()
    {
        //var colorId = isWhite ? Resource.Color.white_space_overlay_color : Resource.Color.black_space_overlay_color;
        /////<returns> </returns>A single color value in the form 0xAARRGGBB.
        //var colorARGB = ContextCompat.GetColor(context, colorId);

        space.SetImageLevel(0);
    }


    public BoardSpace Forward(Dictionary<(char, int), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Down(board);
        return this.Up(board);
    }

    public BoardSpace Backward(Dictionary<(char, int), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Up(board);
        return this.Down(board);
    }

    public BoardSpace DiagonalUp(Dictionary<(char, int), BoardSpace> board, bool isRight)
    {
        BoardSpace up = Up(board);
        if (up == null)
            return null;

        if (isRight)
        {
            BoardSpace right = up.Right(board);
            if (right == null)
                return null;

            return right;
        }

        BoardSpace left = up.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public BoardSpace DiagonalDown(Dictionary<(char, int), BoardSpace> board, bool isRight)
    {
        BoardSpace down = Down(board);
        if (down == null)
            return null;

        if (isRight)
        {
            BoardSpace right = down.Right(board);
            if (right == null)
                return null;

            return right;
        }

        BoardSpace left = down.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public BoardSpace Up(Dictionary<(char, int), BoardSpace> board)
    {
        (char rank, int file) = GetBoardIndex();
        file++;
        (char, int) index = (rank, file);
        if (!board.ContainsKey(index))
            return null;

        return board[index];
    }

    public BoardSpace Down(Dictionary<(char, int), BoardSpace> board)
    {
        (char rank, int file) = GetBoardIndex();
        file--;
        (char, int) index = (rank, file);
        if (!board.ContainsKey(index))
            return null;

        return board[index];
    }

    public BoardSpace Right(Dictionary<(char, int), BoardSpace> board)
    {
        (char rank, int file) = GetBoardIndex();
        rank++;
        (char, int) index = (rank, file);
        if (!board.ContainsKey(index))
            return null;

        return board[index];
    }

    public BoardSpace Left(Dictionary<(char, int), BoardSpace> board)
    {
        (char rank, int file) = GetBoardIndex();
        rank--;
        (char, int) index = (rank, file);
        if (!board.ContainsKey(index))
            return null;

        return board[index];
    }

    public Piece GetPiece(Dictionary<(string, int), Piece> boardPieces)
    {
        var pieces = boardPieces.Values.Where(p => p.spaceId == this.spaceId).ToList();
        if (pieces.Count <= 0)
            return null;

        return boardPieces.Values.FirstOrDefault(p => p.spaceId == this.spaceId);
    }

    public BoardSpace GetBoardSpace(Dictionary<(char, int), BoardSpace> board) => board[this.GetBoardIndex()];

    public (char, int) GetBoardIndex()
    {
        string space = res.GetResourceName(this.spaceId).Split("__")[1];
        //s = "A1";  s[0]='A'                   s[^1]='1'
        Log.Error("DebugCatSpace", $"[{space[0]}], [{space[^1]}]");
        return (space[0], int.Parse($"{space[^1]}"));
    }

    public override string ToString()
    {
        string resourceName = res.GetResourceName(spaceId);
        return resourceName.Split("__")[1];
    }
}
