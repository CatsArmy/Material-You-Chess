using Android.Util;
using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

[Serializable]
public abstract class Piece : BoardSpace, IPiece
{
    [NonSerialized]
    public ImageView? piece;
    public int id;

    public Piece(ImageView? piece, int id, ImageView? space, bool isWhite, int spaceId, Action? callback) : base(space, isWhite, spaceId)
    {
        this.piece = piece;
        this.id = id;
        //this.callback = callback;
        space.Clickable = false;
    }

    public abstract List<Move> Moves(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces);

    public virtual void Update() { }

    public void Move(BoardSpace? destination)
    {
        base.spaceId = destination.spaceId;
        base.space = destination?.space;
        ConstraintLayout.LayoutParams? @params = this.piece?.LayoutParameters as ConstraintLayout.LayoutParams;
        @params.TopToTop = destination.spaceId;
        @params.BottomToBottom = destination.spaceId;
        @params.StartToStart = destination.spaceId;
        @params.EndToEnd = destination.spaceId;
        @params.VerticalBias = 0.5f;
        @params.HorizontalBias = 0.5f;

        @params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        @params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        @params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        @params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        this.piece.LayoutParameters = @params;
        this.piece.RequestLayout();
    }

    public (int, ImageView?) FakeMove(int spaceId, ImageView spaceView)
    {
        var lastSpaceView = base.space;
        var lastSpaceId = base.spaceId;
        base.spaceId = spaceId;
        base.space = spaceView;
        return (lastSpaceId, lastSpaceView);
    }

    public void Capture(Piece destination, Dictionary<(string, int), Piece> pieces)
    {
        //logic
        //check for if move would result in a check
        var rip = destination.GetPieceIndex();
        pieces.Remove(rip);
        destination.piece.Visibility = Android.Views.ViewStates.Gone;
        destination.piece.Enabled = false;
        destination.piece.Clickable = false;

        this.Move(destination);
        //add to the list of captured pieces
    }

    public void Diagonals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.DiagonalsUpRight(board, pieces, ref moves);
        this.DiagonalsUpLeft(board, pieces, ref moves);
        this.DiagonalsDownRight(board, pieces, ref moves);
        this.DiagonalsDownLeft(board, pieces, ref moves);
    }

    public void DiagonalsUpRight(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (BoardSpace? diagonal = this.DiagonalUp(board, true); diagonal != null; diagonal = diagonal.DiagonalUp(board, true))
        {
            if (diagonal == null)
                break;
            Piece? piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsUpLeft(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (BoardSpace? diagonal = this.DiagonalUp(board, false); diagonal != null; diagonal = diagonal.DiagonalUp(board, false))
        {
            if (diagonal == null)
                break;
            Piece? piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsDownRight(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (BoardSpace? diagonal = this.DiagonalDown(board, true); diagonal != null; diagonal = diagonal.DiagonalDown(board, true))
        {
            if (diagonal == null)
                break;
            Piece? piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void DiagonalsDownLeft(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        for (BoardSpace? diagonal = this.DiagonalDown(board, false); diagonal != null; diagonal = diagonal.DiagonalDown(board, false))
        {
            if (diagonal == null)
                break;
            Piece? piece = diagonal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(diagonal, true));
                break;
            }

            moves.Add(new Move(diagonal));
        }
    }

    public void Horizontals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.Horizontals(board, pieces, true, ref moves);
        this.Horizontals(board, pieces, false, ref moves);
    }

    public void Horizontals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, bool isRight, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), BoardSpace>, BoardSpace> iterator = isRight ? this.Right : this.Left;
        for (BoardSpace? horizontal = iterator(board); horizontal != null; iterator = isRight ? horizontal.Right
            : horizontal.Left, horizontal = iterator(board))
        {
            if (horizontal == null)
                break;
            Piece? piece = horizontal.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(horizontal, true));
                break;
            }

            moves.Add(new Move(horizontal));
        }
    }

    public void Verticals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, ref List<Move> moves)
    {
        this.Verticals(board, pieces, true, ref moves);
        this.Verticals(board, pieces, false, ref moves);
    }

    public void Verticals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces, bool isUp, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), BoardSpace>, BoardSpace> iterator = isUp ? this.Up : this.Down;
        for (BoardSpace? vertical = iterator(board); vertical != null; iterator = isUp ? vertical.Up
            : vertical.Down, vertical = iterator(board))
        {
            if (vertical == null)
                break;
            Piece? piece = vertical.GetPiece(pieces);
            if (piece != null)
            {
                if (piece.isWhite != this.isWhite)
                    moves.Add(new Move(vertical, true));
                break;
            }

            moves.Add(new Move(vertical));
        }
    }

    public (BoardSpace?, BoardSpace?) KnightMovesUp(Dictionary<(char, int), BoardSpace> board)
    {
        var up = this.Up(board);
        if (up == null)
            return (null, null);
        up = up.Up(board);
        if (up == null)
            return (null, null);
        return (up.Right(board), up.Left(board));
    }

    public (BoardSpace?, BoardSpace?) KnightMovesDown(Dictionary<(char, int), BoardSpace> board)
    {
        var down = this.Down(board);
        if (down == null)
            return (null, null);
        down = down.Down(board);
        if (down == null)
            return (null, null);
        return (down.Right(board), down.Left(board));
    }

    public (BoardSpace?, BoardSpace?) KnightMovesRight(Dictionary<(char, int), BoardSpace> board)
    {
        var right = this.Right(board);
        if (right == null)
            return (null, null);
        right = right.Right(board);
        if (right == null)
            return (null, null);
        return (right.Up(board), right.Down(board));
    }

    public (BoardSpace?, BoardSpace?) KnightMovesLeft(Dictionary<(char, int), BoardSpace> board)
    {
        var left = this.Left(board);
        if (left == null)
            return (null, null);
        left = left.Left(board);
        if (left == null)
            return (null, null);
        return (left.Up(board), left.Down(board));
    }

    public (string, int) GetPieceIndex()
    {
        string? resourceName = res?.GetResourceName(this.id);
        string? piece = resourceName?.Split("__")[1];
        //     [0..^1] = "bPawn"        012345      [^1]='1'
        //                         gmp__bPawn1 | 6 
        //                              543210
        Log.Error("DebugCatPiece", $"[{piece[0..^1]}], [{piece[^1]}]");
        return (piece[0..^1], int.Parse($"{piece[^1]}"));
    }

    public override string? ToString()
    {
        string? str1 = res?.GetResourceName(this.id);
        string? str2 = res?.GetResourceName(this.spaceId);
        return $"[{str1?.Split("__")[1]}], [{str2?.Split("__")[1]}]";
    }
}
