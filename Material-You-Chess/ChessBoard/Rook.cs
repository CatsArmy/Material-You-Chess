using System;
using System.Collections.Generic;
using Android.Widget;

namespace Chess.ChessBoard;

[Serializable]
public class Rook : Piece
{
    public bool HasMoved = false;
    public Rook(ImageView piece, int id, ImageView space, bool isWhite, int spaceId, Action callback) : base(piece, id, space, isWhite, spaceId, callback) { }
    public override List<Move> Moves(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        this.Horizontals(board, pieces, ref moves);
        this.Verticals(board, pieces, ref moves);
        return moves;
    }
}
