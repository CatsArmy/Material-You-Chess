using System;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Util;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;


public interface IPiece
{
    public abstract bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces);
    public abstract List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces);
}

public abstract class Piece : Space, IPiece
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

    public abstract List<Move> Moves(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces);
    public virtual bool Move(Space dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        base.spaceId = dest.spaceId;
        base.space = dest.space;
        this.piece.RequestLayout();
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
        this.piece.RequestLayout();

        return true;
    }

    public virtual bool Capture(Piece dest, Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        //check for if move would result in a check
        var rip = dest.GetPieceIndex();
        pieces.Remove(rip);
        dest.piece.Visibility = Android.Views.ViewStates.Gone;
        dest.piece.Enabled = false;
        dest.piece.Clickable = false;
        //dest.piece.Touchables.Clear();
        //var @params = dest.piece.LayoutParameters as ConstraintLayout.LayoutParams;
        //@params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        //@params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        //@params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        //@params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        //@params.Height = 0;
        //@params.Width = 0;
        //dest.piece.LayoutParameters = @params;
        //dest.piece.Elevation = -999;

        this.Move(dest, board, pieces);
        //add to the list of captured pieces
        return true;
    }

    public void Diagonals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.DiagonalsUpRight(board, pieces, ref moves);
        this.DiagonalsUpLeft(board, pieces, ref moves);
        this.DiagonalsDownRight(board, pieces, ref moves);
        this.DiagonalsDownLeft(board, pieces, ref moves);
    }

    //public void Diagonals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, bool isRight, bool isUp, ref List<Move> moves)
    //{
    //    Func<Dictionary<(char, int), Space>, Space> iterator = isUp ? this.Up : this.Down;
    //    for (Space horizantal = iterator(board); horizantal != null; horizantal = iterator(board))
    //    {
    //        Piece piece = horizantal.GetPiece(pieces);
    //        if (piece != null)
    //        {
    //            if (piece.isWhite != this.isWhite)
    //                moves.Add(new Move(horizantal, true));
    //            break;
    //        }

    //        moves.Add(new Move(horizantal));
    //        iterator = isUp switch
    //        {
    //            true => horizantal.Up,
    //            false => horizantal.Down,
    //        };
    //    }
    //}

    public void DiagonalsUpRight(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (Space diagonal = this.DiagonalUp(board, true); diagonal != null; diagonal = diagonal.DiagonalUp(board, true))
        {
            if (diagonal == null)
                break;
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
            if (diagonal == null)
                break;
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
            if (diagonal == null)
                break;
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
            if (diagonal == null)
                break;
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

    public void Horizontals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.Horizontals(board, pieces, true, ref moves);
        this.Horizontals(board, pieces, false, ref moves);
    }

    public void Horizontals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, bool isRight, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), Space>, Space> iterator = isRight ? this.Right : this.Left;
        for (Space horizantal = iterator(board); horizantal != null; iterator = isRight ? horizantal.Right
            : horizantal.Left, horizantal = iterator(board))
        {
            if (horizantal == null)
                break;
            Piece piece = horizantal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(horizantal, true));
                break;
            }

            moves.Add(new Move(horizantal));
        }
    }

    public void Verticals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.Verticals(board, pieces, true, ref moves);
        this.Verticals(board, pieces, false, ref moves);
    }

    public void Verticals(Dictionary<(char, int), Space> board, Dictionary<(string, int), Piece> pieces, bool isUp, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), Space>, Space> iterator = isUp ? this.Up : this.Down;
        for (Space vertical = iterator(board); vertical != null; iterator = isUp ? vertical.Up
            : vertical.Down, vertical = iterator(board))
        {
            if (vertical == null)
                break;
            Piece piece = vertical.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(vertical, true));
                break;
            }

            moves.Add(new Move(vertical));
        }
    }

    public (string, int) GetPieceIndex()
    {
        string resourceName = res.GetResourceName(id);
        string piece = resourceName.Split("__")[1];
        //     [0..^1] = "bPawn"        012345      [^1]='1'
        //                         gmp__bPawn1 | 6 
        //                              543210
        Log.Error("DebugCatPiece", $"[{piece[0..^1]}], [{piece[^1]}]");
        return (piece[0..^1], int.Parse($"{piece[^1]}"));
    }

    public override string ToString()
    {
        string str1 = res.GetResourceName(id);
        string str2 = res.GetResourceName(spaceId);
        return $"[{str1.Split("__")[1]}], [{str2.Split("__")[1]}]";
    }
}
