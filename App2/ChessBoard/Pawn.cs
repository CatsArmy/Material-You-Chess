using System.Collections.Generic;
using System.Linq;
using Android.Widget;

namespace Chess.ChessBoard;

public class Pawn : Piece
{
    private bool isFirstMove = true;

    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;

        var moves = GetMoves(board, pieces);
        var move = moves.FirstOrDefault(i => i.Space.spaceId == dest.spaceId);
        if (move == null)
            return false;
        isFirstMove = false;
        if (!isLegalMove)
            return false;
        //promote logic
        return base.Move(dest, board, pieces);
    }

    public List<Move> GetMoves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        var (rank, file) = board.FirstOrDefault(s => s.Value.spaceId == spaceId).Key;
        int i = this.isWhite switch
        {
            true => i = file + 1,
            false => i = file - 1,
        };

        char j = char.Parse($"{rank}");
        char k = char.Parse($"{rank}");
        j++;
        k--;

        Space move = board[(rank, i)];
        if (pieces.Values.FirstOrDefault(p => p.spaceId == move.spaceId) == null)
            moves.Add(new(move, false));

        if (isFirstMove)
        {
            Space space = board[(rank, this.isWhite switch
            {
                true => i + 1,
                false => i - 1,
            })];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece == null)
                moves.Add(new(space, false, true));
        }

        if (j < 'H')
        {
            Space space = board[(j, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
        }

        if (k >= 'A')
        {
            Space space = board[(k, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
        }
        return moves;
    }



    public void Promote()
    {
        //Get name via id
        //create a (string,int) key with the name
        //remove the value from the dict with the key
        //Open Promote dialog/popup thingy
        //add back the value but as a the selected piece(cant be king)
    }
}
