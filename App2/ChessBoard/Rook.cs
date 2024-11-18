using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

public class Rook : Piece
{
    public Rook(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        return true;
    }
}
