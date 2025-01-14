﻿using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using Chess.Util.Logger;

namespace Chess.ChessBoard;

/*[DataContract]*/
public class BoardPiece(int id, (string, int) index, char abbreviation, bool isWhite, ISpace space, ConstraintLayout App) : IPiece
{
    /*[IgnoreDataMember]*/
    public ImageView? Piece { get; set; } = App.FindViewById<ImageView>(id);

    /*[DataMember]*/
    public ISpace Space { get; set; } = space;

    /*[DataMember]*/
    public int Id { get; } = id;

    /*[DataMember]*/
    public bool IsWhite { get; } = isWhite;

    /*[DataMember]*/
    public (string, int) Index { get; } = index;

    /*[DataMember]*/
    public char Abbreviation { get; set; } = abbreviation;

    public virtual void Update() { }

    public virtual List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces) => [];

    public virtual void Move(ISpace destination)
    {
        this.Space = destination;
        if (this.Piece?.LayoutParameters is not ConstraintLayout.LayoutParams @params)
        {
            Log.Warn("Piece layout params are not the correct type");
            return;
        }
        const float Half = 0.5f;
        @params.VerticalBias = Half;
        @params.HorizontalBias = Half;
        @params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        @params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        @params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        @params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        @params.TopToTop = destination.Id;
        @params.BottomToBottom = destination.Id;
        @params.StartToStart = destination.Id;
        @params.EndToEnd = destination.Id;
        this.Piece.LayoutParameters = @params;
        this.Piece.RequestLayout();
    }

    public void Capture(IPiece destination, Dictionary<(string, int), IPiece> pieces)
    {
        Log.Debug("Capture");
        //logic
        //check for if move would result in a check
        destination.Piece!.Visibility = Android.Views.ViewStates.Gone;
        destination.Piece!.Enabled = false;
        pieces.Remove(destination.Index);
        destination.Piece!.Clickable = false;

        if (destination.Piece is View view)
            if (view.Parent is ViewGroup parent)
                parent.RemoveView(view);

        //this.Move(destination.Space);
        //add to the list of captured pieces
    }

    public void Diagonals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        this.DiagonalsUpRight(board, pieces, ref moves);
        this.DiagonalsUpLeft(board, pieces, ref moves);
        this.DiagonalsDownRight(board, pieces, ref moves);
        this.DiagonalsDownLeft(board, pieces, ref moves);
    }

    public void DiagonalsUpRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        for (ISpace? diagonal = this.Space.DiagonalUp(board, true); diagonal != null; diagonal = diagonal.DiagonalUp(board, true))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is IPiece diagonalPiece)
            {
                if (diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsUpLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        for (ISpace? diagonal = this.Space.DiagonalUp(board, false); diagonal != null; diagonal = diagonal.DiagonalUp(board, false))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is IPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsDownRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        for (ISpace? diagonal = this.Space.DiagonalDown(board, true); diagonal != null; diagonal = diagonal.DiagonalDown(board, true))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is IPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsDownLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        for (ISpace? diagonal = this.Space.DiagonalDown(board, false); diagonal != null; diagonal = diagonal.DiagonalDown(board, false))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is IPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        this.Horizontals(board, pieces, true, ref moves);
        this.Horizontals(board, pieces, false, ref moves);
    }

    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isRight, ref List<IMove> moves)
    {
        Func<Dictionary<(char, int), ISpace>, ISpace?> iterator = isRight ? this.Space.Right : this.Space.Left;
        for (ISpace? horizontal = iterator(board); horizontal != null; iterator = isRight ? horizontal.Right
            : horizontal.Left, horizontal = iterator(board))
        {
            if (horizontal == null)
                break;
            if (horizontal.Piece(pieces) is IPiece horizontalPiece)
            {
                if (horizontalPiece is not null && horizontalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, horizontalPiece));
                break;
            }

            moves.Add(new Move(this, horizontal));
        }
    }

    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves)
    {
        this.Verticals(board, pieces, true, ref moves);
        this.Verticals(board, pieces, false, ref moves);
    }

    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isUp, ref List<IMove> moves)
    {
        Func<Dictionary<(char, int), ISpace>, ISpace?> iterator = isUp ? this.Space.Up : this.Space.Down;
        for (ISpace? vertical = iterator(board); vertical != null; iterator = isUp ? vertical.Up
            : vertical.Down, vertical = iterator(board))
        {
            if (vertical == null)
                break;
            if (vertical.Piece(pieces) is IPiece verticalPiece)
            {
                if (verticalPiece is not null && verticalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, verticalPiece));
                break;
            }

            moves.Add(new Move(this, vertical));
        }
    }

    public (ISpace?, ISpace?) DiagonalMovesUp(Dictionary<(char, int), ISpace> board)
    {
        var up = this.Space.Up(board);
        if (up == null)
            return (null, null);
        up = up.Up(board);
        if (up == null)
            return (null, null);
        return (up.Right(board), up.Left(board));
    }

    public (ISpace?, ISpace?) DiagonalMovesDown(Dictionary<(char, int), ISpace> board)
    {
        var down = this.Space.Down(board);
        if (down == null)
            return (null, null);
        down = down.Down(board);
        if (down == null)
            return (null, null);
        return (down.Right(board), down.Left(board));
    }

    public (ISpace?, ISpace?) DiagonalMovesRight(Dictionary<(char, int), ISpace> board)
    {
        var right = this.Space.Right(board);
        if (right == null)
            return (null, null);
        right = right.Right(board);
        if (right == null)
            return (null, null);
        return (right.Up(board), right.Down(board));
    }

    public (ISpace?, ISpace?) DiagonalMovesLeft(Dictionary<(char, int), ISpace> board)
    {
        var left = this.Space.Left(board);
        if (left == null)
            return (null, null);
        left = left.Left(board);
        if (left == null)
            return (null, null);
        return (left.Up(board), left.Down(board));
    }

    //public override string? ToString() => $"[{this.Space}], [{ChessActivity.Instance?.Resources?.GetResourceName(this.Id)?.Split("__")[1]}]";
}
