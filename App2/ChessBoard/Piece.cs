using System.Collections.Generic;
using Android.Content.Res;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

public abstract class Piece : Space
{
    public ImageView piece;
    public int id;


    public Piece(ImageView piece, int id, ImageView space, bool isWhite, int spaceId) : base(space, isWhite, spaceId)
    {
        this.piece = piece;
        this.id = id;
        space.Clickable = false;
    }

    public static void SetResources(Resources _res)
    {
        Space.res = _res;
    }

    public virtual bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        base.spaceId = dest.spaceId;
        base.space = dest.space;

        ConstraintLayout.LayoutParams @params = this.piece.LayoutParameters as ConstraintLayout.LayoutParams;
        @params.TopToTop = dest.spaceId;
        @params.BottomToBottom = dest.spaceId;
        @params.StartToStart = dest.spaceId;
        @params.EndToEnd = dest.spaceId;
        @params.VerticalBias = 0.5f;
        @params.HorizontalBias = 0.5f;

        @params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        @params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        @params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        @params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        this.piece.LayoutParameters = @params;

        return true;
    }


    public virtual bool Capture(Piece dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        return true;
    }

    public void DiagonalsUpRight(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (Space diagonal = this.DiagonalUp(board, true); diagonal != null; diagonal = diagonal.DiagonalUp(board, true))
        {
            Piece piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsUpLeft(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (Space diagonal = this.DiagonalUp(board, false); diagonal != null; diagonal = diagonal.DiagonalUp(board, false))
        {
            Piece piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsDownRight(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {

        for (Space diagonal = this.DiagonalDown(board, true); diagonal != null; diagonal = diagonal.DiagonalDown(board, true))
        {
            Piece piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsDownLeft(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (Space diagonal = this.DiagonalDown(board, false); diagonal != null; diagonal = diagonal.DiagonalDown(board, false))
        {
            Piece piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public Space Forward(Dictionary<(char, int), Space> board)
    {
        if (!this.isWhite)
            return this.Down(board);
        return this.Up(board);
    }

    public Space Backward(Dictionary<(char, int), Space> board)
    {
        if (!this.isWhite)
            return this.Up(board);
        return this.Down(board);
    }

    public (string, int) GetPieceIndex()
    {
        string resourceName = res.GetResourceName(id);
        string piece = resourceName.Split("__")[1];
        //gmp__bPawn1
        // ^1 = '1' & 0..^2 = bPawn
        return (piece[0..^2], int.Parse($"{piece[^1]}"));
    }

    public override string ToString()
    {
        string str1 = res.GetResourceName(id);
        string str2 = res.GetResourceName(spaceId);
        return $"[{str1.Split("__")[1]}], [{str2.Split("__")[1]}]";
    }
}
