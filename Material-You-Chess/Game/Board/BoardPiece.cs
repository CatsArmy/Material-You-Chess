using System.Text.Json.Serialization;
using AndroidX.ConstraintLayout.Widget;
using Chess.App;
using Chess.App.Common;
using Chess.Game.Moves;
using Google.Android.Material.Button;

namespace Chess.Game.Board;

[JsonPolymorphic()]
[JsonDerivedType(typeof(BoardPiece), nameof(BoardPiece))]
[JsonDerivedType(typeof(SpecialPiece), nameof(SpecialPiece))]
[JsonDerivedType(typeof(Knight), nameof(Knight))]
[JsonDerivedType(typeof(Bishop), nameof(Bishop))]
[JsonDerivedType(typeof(Queen), nameof(Queen))]
[JsonDerivedType(typeof(King), nameof(King))]
[JsonDerivedType(typeof(Pawn), nameof(Pawn))]
[JsonDerivedType(typeof(Rook), nameof(Rook))]
[JsonDerivedType(typeof(WhiteKing), nameof(WhiteKing))]
[JsonDerivedType(typeof(WhitePawn), nameof(WhitePawn))]
[JsonDerivedType(typeof(WhiteRook), nameof(WhiteRook))]
[JsonDerivedType(typeof(WhiteQueen), nameof(WhiteQueen))]
[JsonDerivedType(typeof(WhiteBishop), nameof(WhiteBishop))]
[JsonDerivedType(typeof(WhiteKnight), nameof(WhiteKnight))]
[JsonDerivedType(typeof(BlackKnight), nameof(BlackKnight))]
[JsonDerivedType(typeof(BlackBishop), nameof(BlackBishop))]
[JsonDerivedType(typeof(BlackQueen), nameof(BlackQueen))]
[JsonDerivedType(typeof(BlackKing), nameof(BlackKing))]
[JsonDerivedType(typeof(BlackPawn), nameof(BlackPawn))]
[JsonDerivedType(typeof(BlackRook), nameof(BlackRook))]
public class BoardPiece(MaterialButton PieceView, BoardSpace space)
{
    private static readonly ConstraintLayout? BoardLayout = IChessActivity.Instance?.BoardLayout;
    public BoardPiece(int id, BoardSpace space) : this(BoardLayout!.FindViewById<MaterialButton>(id)!, space) { }

    public int Id { get; } = PieceView.Id;
    public BoardSpace Space { get; set; } = space;
    [JsonIgnore] public BoardSpace? LastSpace { get; set; }
    [JsonIgnore] public MaterialButton? PieceView { get; set; } = PieceView;
    public (string prefix, int count) Index => (this.Prefix, this.Count);
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public virtual string Prefix { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public virtual int Count { get; }
    public virtual bool IsWhite { get; }
    public virtual char Abbreviation { get; }

    public virtual void Update() { return; }

    /// <summary>An overridable virtual method that generates the available moves at this state of the <paramref name="game"/></summary>
    /// <returns>a list of available moves based on the rules of the chess game</returns>
    public virtual List<Move> Moves(ChessGame game) => [];

    public virtual void Move(Move move, ChessGame game)
    {
        if (move is Capture capture)
        {
            this.Capture(capture.Piece, game);
        }

        this.Move(move);
    }

    /// <remarks> Visually moves the piece </remarks>
    /// <summary> <see langword="this"/> is the <see cref="BoardPiece"/> that will be moved to the <paramref name="move"/> </summary>
    public void Move(Move move)
    {
        this.LastSpace = this.Space;
        this.Space = move.Destination;
        if (this.PieceView?.Parent is not ConstraintLayout parent)
        {
            Logger.Warn("PieceView is not in a ConstraintLayout parent");
            return;
        }

        var constraintSet = new ConstraintSet();
        constraintSet.Clone(parent);

        int pieceViewId = this.PieceView.Id;
        int destinationId = this.Space.SpaceView!.Id;

        // Center the piece within the destination space
        constraintSet.Connect(pieceViewId, ConstraintSet.Top, destinationId, ConstraintSet.Top);
        constraintSet.Connect(pieceViewId, ConstraintSet.Bottom, destinationId, ConstraintSet.Bottom);
        constraintSet.Connect(pieceViewId, ConstraintSet.Start, destinationId, ConstraintSet.Start);
        constraintSet.Connect(pieceViewId, ConstraintSet.End, destinationId, ConstraintSet.End);

        // Apply the updated constraints
        constraintSet.ApplyTo(parent);
        this.PieceView.RequestLayout();
    }

    /// <summary> <see langword="this"/> <see cref="BoardPiece"/> is the <see cref="Move.Origin"/> </summary>
    /// <remarks> <see langword="this"/> is not the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </remarks>
    /// <param name="destination"> is the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </param>
    public virtual void Capture(BoardPiece destination, ChessGame game) => destination.Capture(game);

    /// <summary> <see langword="this"/> <see cref="BoardPiece"/> is the <see cref="Move.Destination"/> </summary>
    /// <remarks> <see langword="this"/> is the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </remarks>
    public virtual void Capture(ChessGame game)
    {
        game.AllPieces.Remove(this.Index);
        game.Player!.Pieces.Remove(this.Index);
        this.PieceView!.Enabled = false;
        this.PieceView!.Clickable = false;
        this.PieceView!.Visibility = Android.Views.ViewStates.Gone;
    }

    public void Diagonals(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
    {
        this.DiagonalsUpRight(board, pieces, ref moves);
        this.DiagonalsUpLeft(board, pieces, ref moves);
        this.DiagonalsDownRight(board, pieces, ref moves);
        this.DiagonalsDownLeft(board, pieces, ref moves);
    }

    public void DiagonalsUpRight(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
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

    public void DiagonalsUpLeft(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
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

    public void DiagonalsDownRight(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
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

    public void DiagonalsDownLeft(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
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

    public void Horizontals(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
    {
        this.Horizontals(board, pieces, true, ref moves);
        this.Horizontals(board, pieces, false, ref moves);
    }

    public void Horizontals(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, bool isRight, ref List<Move> moves)
    {
        Func<Dictionary<(char file, int rank), BoardSpace>, BoardSpace?> iterator = isRight ? this.Space.Right : this.Space.Left;
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

    public void Verticals(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, ref List<Move> moves)
    {
        this.Verticals(board, pieces, true, ref moves);
        this.Verticals(board, pieces, false, ref moves);
    }

    public void Verticals(Dictionary<(char file, int rank), BoardSpace> board, Dictionary<(string Prefix, int Count), BoardPiece> pieces, bool isUp, ref List<Move> moves)
    {
        Func<Dictionary<(char file, int rank), BoardSpace>, BoardSpace?> iterator = isUp ? this.Space.Up : this.Space.Down;
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

    public (BoardSpace?, BoardSpace?) DiagonalMovesUp(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var up = this.Space.Up(board);
        if (up == null)
            return (null, null);
        up = up.Up(board);
        if (up == null)
            return (null, null);
        return (up.Right(board), up.Left(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesDown(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var down = this.Space.Down(board);
        if (down == null)
            return (null, null);
        down = down.Down(board);
        if (down == null)
            return (null, null);
        return (down.Right(board), down.Left(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesRight(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var right = this.Space.Right(board);
        if (right == null)
            return (null, null);
        right = right.Right(board);
        if (right == null)
            return (null, null);
        return (right.Up(board), right.Down(board));
    }

    public (BoardSpace?, BoardSpace?) DiagonalMovesLeft(Dictionary<(char file, int rank), BoardSpace> board)
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