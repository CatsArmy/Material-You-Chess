using System.Collections.Generic;
using System.Linq;
using Android.Widget;

namespace Chess.ChessBoard;

public class Bishop : Piece
{
    public Bishop(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(string, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;

        var (a, b) = base.GetSpaceKey();
        int _i = int.Parse($"{b}");
        char j = char.Parse($"{a[0]}");
        j++;
        char k = char.Parse($"{a[0]}");
        k--;
        bool topRight = false;
        bool captureTopLeft = false;
        bool captureTopRight = false;
        bool topLeft = false;

        (string, int) TopLeft;
        (string, int) TopRight;

        (string, int) BottomLeft;
        (string, int) BottomRight;
        for (int i = _i + 1; i <= 8; i++)
        {
            (string, int) left = ($"{j}", i);
            var leftPiece = pieces.Values.FirstOrDefault(p => p.spaceId == board[left].spaceId);
            if (leftPiece == null)
            {
                TopLeft = left;
                if (j < 'H' && !topLeft)
                    j++;
                else
                    topLeft = true;
                continue;
            }
            if (leftPiece.isWhite == this.isWhite)
            {
                topLeft = true;
                continue;
            }
            captureTopLeft = true;
            topLeft = true;
            TopLeft = left;
        }

        for (int i = _i + 1; i <= 8; i++)
        {
            (string, int) right = ($"{k}", i);
            var rightPiece = pieces.Values.FirstOrDefault(p => p.spaceId == board[right].spaceId);
            if (rightPiece == null)
            {
                TopRight = right;
                if (k >= 'A' && !topRight)
                    k--;
                else
                    topRight = true;

                continue;
            }
            if (rightPiece.isWhite == this.isWhite)
            {
                topRight = true;
                continue;
            }
            captureTopRight = true;
            topRight = true;
            TopRight = right;
        }

        if (!isLegalMove)
            return false;
        return base.Move(dest, board, pieces);
    }
}
