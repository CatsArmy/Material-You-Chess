using System.Text.Json.Serialization;
using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Interfaces;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class BoardSpace(char File, int Rank, bool IsWhite, ImageView Space) : ISpace
{
    [JsonIgnore] public (char File, int Rank) Index => (this.File, this.Rank);
    [JsonIgnore] public ImageView? SpaceView { get; } = Space;
    public bool IsWhite { get; } = IsWhite;
    public char File { get; } = File;
    public int Rank { get; } = Rank;
    public int Id { get; } = Space.Id;
    [JsonConstructor] public BoardSpace(bool IsWhite, char File, int Rank, int Id) : this(File, Rank, IsWhite, Id) { }
    public BoardSpace(char File, int Rank, bool IsWhite, int Id) : this(File, Rank, IsWhite, BoardLayout!.FindViewById<ImageView>(Id)!) { }
    private static readonly ConstraintLayout? BoardLayout = ChessGame.Instance?.Activity.BoardLayout;

    public const int UnselectSpace = 0;
    public const int UnselectMove = 1;
    public const int SelectSpace = 2;
    public const int SelectMove = 3;

    public void Select()
    {
        if (this.IsSelectedMove() || this.IsUnselectedMove())
        {
            this.SpaceView?.SetImageLevel(SelectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(SelectSpace);
    }

    public void Unselect()
    {
        if (this.IsUnselectedMove() || this.IsSelectedMove())
        {
            this.SpaceView?.SetImageLevel(UnselectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(UnselectSpace);
    }

    public void IndicateMoveable()
    {
        if (this.IsUnselected() || this.IsUnselectedMove())
        {
            this.SpaceView?.SetImageLevel(UnselectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(SelectMove);
    }

    public void IndicateUnmovable()
    {
        if (this.IsUnselected() || this.IsUnselectedMove())
        {
            this.SpaceView?.SetImageLevel(UnselectSpace);
            return;
        }

        this.SpaceView?.SetImageLevel(SelectSpace);
    }

    public bool IsSelectedMove() => this.SpaceView?.Drawable?.Level == SelectMove;
    public bool IsUnselectedMove() => this.SpaceView?.Drawable?.Level == UnselectMove;
    public bool IsUnselected() => this.SpaceView?.Drawable?.Level == UnselectSpace;
    public bool IsSelected() => this.SpaceView?.Drawable?.Level == SelectSpace;

    /// <returns> whether or not an enemy piece can capture a (theoretical) piece 
    /// that would be placed on the given space </returns>
    public bool IsThreateningSpace(ref List<Move> enemy)
    {
        return enemy.FirstOrDefault(move => move.Destination == this) != null;
    }

    public BoardSpace? DiagonalUp(Dictionary<(char file, int rank), BoardSpace> board, bool isRight)
    {
        var up = this.Up(board);
        if (up == null)
            return null;

        if (isRight)
        {
            var right = up.Right(board);
            if (right == null)
                return null;

            return right;
        }

        var left = up.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public BoardSpace? DiagonalDown(Dictionary<(char file, int rank), BoardSpace> board, bool isRight)
    {
        var down = Down(board);
        if (down == null)
            return null;

        if (isRight)
        {
            var right = down.Right(board);
            if (right == null)
                return null;

            return right;
        }

        var left = down.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public BoardSpace? Up(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, ++Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Down(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, --Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Right(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((++File, Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Left(Dictionary<(char file, int rank), BoardSpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((--File, Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Forward(Dictionary<(char file, int rank), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Down(board);
        return this.Up(board);
    }

    public BoardSpace? Backward(Dictionary<(char file, int rank), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Up(board);
        return this.Down(board);
    }

    public BoardPiece? Piece(ChessGame game) => this.Piece(game.AllPieces);
    public BoardPiece? Piece(Dictionary<(string Prefix, int Count), BoardPiece> boardPieces)
        => boardPieces.Values.FirstOrDefault(p => p.Space.Index == this.Index);
}
