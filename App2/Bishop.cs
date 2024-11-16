using System.Collections.Generic;
using Android.Widget;

namespace Chess;

public class Bishop : BoardPiece
{
    public Bishop(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(BoardSpace dest, Dictionary<(string, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces)
    {
        return true;
    }
}
