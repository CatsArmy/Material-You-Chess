using System.Collections.Generic;
using Android.Widget;

namespace Chess;

public class Knight : BoardPiece
{
    public Knight(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(BoardSpace dest, Dictionary<(string, int), BoardSpace> board)
    {
        return false;
    }
}
