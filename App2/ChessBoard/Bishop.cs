using System.Collections.Generic;
using System.Linq;
using Android.Widget;

namespace Chess.ChessBoard;

public class Move
{
    public Move(Space space, bool capture, bool enPassantCapturable = false)
    {
        Space = space;
        Capture = capture;
        EnPassantCapturable = enPassantCapturable;
    }
    public Space Space;
    public bool Capture;
    public bool EnPassantCapturable = false;
}

public class Bishop : Piece
{
    public Bishop(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(piece, id, space, isWhite, spaceId) { }

    public override bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        bool isLegalMove = true;
        var moves = GetMoves(board, pieces);
        var move = moves.FirstOrDefault(i => i.Space.spaceId == dest.spaceId);
        if (move == null)
            return false;
        if (!isLegalMove)
            return false;
        return base.Move(dest, board, pieces);
    }
    public List<Move> GetMoves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        var (rank, file) = board.FirstOrDefault(s => s.Value.spaceId == spaceId).Key;
        int _i = int.Parse($"{file}");

        char j = char.Parse($"{rank}");
        j++;

        for (int i = _i + 1; i <= 8; i++)
        {
            Space space = board[(j, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
                break;
            }
            moves.Add(new(space, false));
            if (j >= 'H')
                break;
            j++;
        }

        char k = char.Parse($"{rank}");
        k--;

        for (int i = _i; i <= 8; i++)
        {
            Space space = board[(k, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
                break;
            }
            moves.Add(new(space, false));
            if (k < 'A')
                break;
            k++;
        }

        j = char.Parse($"{rank}");
        j++;

        for (int i = _i - 1; i <= 1; i--)
        {
            Space space = board[(j, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
                break;
            }
            moves.Add(new(space, false));
            if (j >= 'H')
                break;
            j++;
        }


        k = char.Parse($"{rank}");
        k--;

        for (int i = _i - 1; i <= 1; i--)
        {
            Space space = board[(k, i)];
            Piece piece = pieces.Values.FirstOrDefault(p => p.spaceId == space.spaceId);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new(space, true));
                break;
            }
            moves.Add(new(space, false));
            if (k < 'A')
                break;
            k--;
        }
        return moves;
    }
}
