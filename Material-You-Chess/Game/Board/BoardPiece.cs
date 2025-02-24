using System.Text.Json.Serialization;
using Android.Animation;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Game.Moves;

namespace Chess.Game.Board;

[JsonPolymorphic()]
[JsonDerivedType(typeof(King), nameof(King))]
[JsonDerivedType(typeof(Pawn), nameof(Pawn))]
[JsonDerivedType(typeof(Rook), nameof(Rook))]
[JsonDerivedType(typeof(Queen), nameof(Queen))]
[JsonDerivedType(typeof(Knight), nameof(Knight))]
[JsonDerivedType(typeof(Bishop), nameof(Bishop))]
[JsonDerivedType(typeof(SpecialPiece), nameof(SpecialPiece))]
[JsonDerivedType(typeof(BoardPiece), nameof(BoardPiece))]
public class BoardPiece(int id, (string, int) index, char abbreviation, bool isWhite, BoardSpace space, ConstraintLayout boardLayout) : IPiece
{
    [JsonIgnore] public ImageView? Piece { get; set; } = boardLayout.FindViewById<ImageView>(id);

    public BoardSpace Space { get; set; } = space;

    [JsonIgnore] public int Id { get; } = id;

    public bool IsWhite { get; } = isWhite;

    public (string, int) Index { get; } = index;

    public char Abbreviation { get; } = abbreviation;

    public virtual void Update(bool IsUpdatingPlayer = false) { return; }

    public virtual List<Move> Moves(ChessGame game) => [];

    public virtual void Move(Move move, ChessGame game)
    {
        if (game.Player == null || game.Enemy == null)
            return;

        game.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);

        if (move is Capture capture)
        {
            this.Capture(capture.Piece, game);
        }
        this.Move(move);
        game.NextTurn(move);
    }

    internal void Move(Move move)
    {
        this.Space = move.Destination;
        if (this.Piece?.LayoutParameters is not ConstraintLayout.LayoutParams @params)
        {
            Logger.Warn("Piece layout params are not the correct type");
            return;
        }

        const float Center = 0.5f;
        @params.VerticalBias = Center;
        @params.HorizontalBias = Center;
        @params.TopToBottom = ConstraintLayout.LayoutParams.Unset;
        @params.BottomToTop = ConstraintLayout.LayoutParams.Unset;
        @params.StartToEnd = ConstraintLayout.LayoutParams.Unset;
        @params.EndToStart = ConstraintLayout.LayoutParams.Unset;
        @params.TopToTop = this.Space.Id;
        @params.BottomToBottom = this.Space.Id;
        @params.StartToStart = this.Space.Id;
        @params.EndToEnd = this.Space.Id;
        this.Piece.LayoutParameters = @params;
        this.Piece.RequestLayout();
    }

    public virtual void Capture(BoardPiece destination, ChessGame game)
    {
        Logger.Debug("Capture");
        var player = (destination.IsWhite) switch
        {
            true => game.White,
            false => game.Black
        };
        game.AllPieces.Remove(destination.Index);
        player!.Pieces.Remove(destination.Index);
        destination.Piece!.Enabled = false;
        destination.Piece!.Clickable = false;
        destination.Piece!.Visibility = Android.Views.ViewStates.Gone;
    }

    public void Diagonals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        this.DiagonalsUpRight(board, pieces, ref moves);
        this.DiagonalsUpLeft(board, pieces, ref moves);
        this.DiagonalsDownRight(board, pieces, ref moves);
        this.DiagonalsDownLeft(board, pieces, ref moves);
    }

    public void DiagonalsUpRight(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalUp(board, true); diagonal != null; diagonal = diagonal.DiagonalUp(board, true))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsUpLeft(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalUp(board, false); diagonal != null; diagonal = diagonal.DiagonalUp(board, false))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsDownRight(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalDown(board, true); diagonal != null; diagonal = diagonal.DiagonalDown(board, true))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void DiagonalsDownLeft(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalDown(board, false); diagonal != null; diagonal = diagonal.DiagonalDown(board, false))
        {
            if (diagonal == null)
                break;
            if (diagonal.Piece(pieces) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    public void Horizontals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        this.Horizontals(board, pieces, true, ref moves);
        this.Horizontals(board, pieces, false, ref moves);
    }

    public void Horizontals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, bool isRight, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), BoardSpace>, BoardSpace?> iterator = isRight ? this.Space.Right : this.Space.Left;
        for (var horizontal = iterator(board); horizontal != null; iterator = isRight ? horizontal.Right
            : horizontal.Left, horizontal = iterator(board))
        {
            if (horizontal == null)
                break;
            if (horizontal.Piece(pieces) is BoardPiece horizontalPiece)
            {
                if (horizontalPiece is not null && horizontalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, horizontalPiece));
                break;
            }

            moves.Add(new Move(this, horizontal));
        }
    }

    public void Verticals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, ref List<Move> moves)
    {
        this.Verticals(board, pieces, true, ref moves);
        this.Verticals(board, pieces, false, ref moves);
    }

    public void Verticals(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), BoardPiece> pieces, bool isUp, ref List<Move> moves)
    {
        Func<Dictionary<(char, int), BoardSpace>, BoardSpace?> iterator = isUp ? this.Space.Up : this.Space.Down;
        for (var vertical = iterator(board); vertical != null; iterator = isUp ? vertical.Up
            : vertical.Down, vertical = iterator(board))
        {
            if (vertical == null)
                break;
            if (vertical.Piece(pieces) is BoardPiece verticalPiece)
            {
                if (verticalPiece is not null && verticalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, verticalPiece));
                break;
            }

            moves.Add(new Move(this, vertical));
        }
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesUp(Dictionary<(char, int), BoardSpace> board)
    {
        var up = this.Space.Up(board);
        if (up == null)
            return (null, null);
        up = up.Up(board);
        if (up == null)
            return (null, null);
        return (up.Right(board), up.Left(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesDown(Dictionary<(char, int), BoardSpace> board)
    {
        var down = this.Space.Down(board);
        if (down == null)
            return (null, null);
        down = down.Down(board);
        if (down == null)
            return (null, null);
        return (down.Right(board), down.Left(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesRight(Dictionary<(char, int), BoardSpace> board)
    {
        var right = this.Space.Right(board);
        if (right == null)
            return (null, null);
        right = right.Right(board);
        if (right == null)
            return (null, null);
        return (right.Up(board), right.Down(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesLeft(Dictionary<(char, int), BoardSpace> board)
    {
        var left = this.Space.Left(board);
        if (left == null)
            return (null, null);
        left = left.Left(board);
        if (left == null)
            return (null, null);
        return (left.Up(board), left.Down(board));
    }
}
