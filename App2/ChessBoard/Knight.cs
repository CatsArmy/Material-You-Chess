﻿using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

public class Knight : Piece
{
    public Knight(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;
        if (!isLegalMove)
            return false;
        return base.Move(dest, board, pieces);
    }
}
