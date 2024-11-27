﻿using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

public class Queen : Piece
{
    public Queen(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        this.Horizantals(board, pieces, ref moves);
        this.Verticals(board, pieces, ref moves);
        this.Diagonals(board, pieces, ref moves);
        return moves;
    }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;
        if (!isLegalMove)
            return false;
        return base.Move(dest, board, pieces);
    }
}
