using System.Text.Json.Serialization;
using Google.Android.Material.ImageView;
using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Board;

public class BoardSpace(char File, int Rank, bool IsWhite, ShapeableImageView Space)
{
    [JsonIgnore] public ShapeableImageView? SpaceView { get; } = Space;
    public (char File, int Rank) Index => (this.File, this.Rank);
    public bool IsWhite { get; } = IsWhite;
    public char File { get; } = File;
    public int Rank { get; } = Rank;
    public int Id { get; } = Space.Id;

    [method: JsonConstructor]
    public BoardSpace(bool IsWhite, char File, int Rank, int Id) : this(File, Rank, IsWhite, Id) { }
    public BoardSpace(char File, int Rank, bool IsWhite, int Id) : this(File, Rank, IsWhite, ChessActivity.Instance?.BoardLayout!.FindViewById<ShapeableImageView>(Id)!) { }

    public const int UnselectSpace = 0;
    public const int UnselectMove = 1;
    public const int SelectSpace = 2;
    public const int SelectMove = 3;

    /// <summary> Visually Indicates that a piece has moved to this space in the last turn or two </summary>
    public void Select()
    {
        if (this.IsSelectedMove() || this.IsUnselectedMove())
        {
            this.SpaceView?.SetImageLevel(SelectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(SelectSpace);
    }

    /// <summary> Visually Indicates that a piece hasn't moved to this space in the last turn or two </summary>
    public void Unselect()
    {
        if (this.IsUnselectedMove() || this.IsSelectedMove())
        {
            this.SpaceView?.SetImageLevel(UnselectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(UnselectSpace);
    }

    /// <summary> Visually Indicates that a piece can move to this space </summary>
    public void IndicateMoveable()
    {
        if (this.IsUnselected() || this.IsUnselectedMove())
        {
            this.SpaceView?.SetImageLevel(UnselectMove);
            return;
        }

        this.SpaceView?.SetImageLevel(SelectMove);
    }

    /// <summary> Visually Indicates that a piece can no longer move to this space </summary>
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

    public BoardSpace? Up(ChessGame game)
    {
        var Rank = this.Rank;
        if (game.Board.TryGetValue((File, ++Rank), out var value))
            return value;

        return null;
    }

    public BoardSpace? Down(ChessGame game)
    {
        var Rank = this.Rank;
        if (game.Board.TryGetValue((File, --Rank), out var value))
            return value;

        return null;
    }

    public BoardSpace? Right(ChessGame game)
    {
        var File = this.File;
        if (game.Board.TryGetValue((++File, Rank), out var value))
            return value;

        return null;
    }

    public BoardSpace? Left(ChessGame game)
    {
        var File = this.File;
        if (game.Board.TryGetValue((--File, Rank), out var value))
            return value;

        return null;
    }

    public BoardSpace? Forward(ChessGame game, bool isWhite) => isWhite ? this.Up(game) : this.Down(game);
    public BoardSpace? Backward(ChessGame game, bool isWhite) => !isWhite ? this.Up(game) : this.Down(game);
    public BoardSpace? DiagonalUpRight(ChessGame game) => this.Up(game)?.Right(game);
    public BoardSpace? DiagonalUpLeft(ChessGame game) => this.Up(game)?.Left(game);
    public BoardSpace? DiagonalDownLeft(ChessGame game) => this.Down(game)?.Left(game);
    public BoardSpace? DiagonalDownRight(ChessGame game) => this.Down(game)?.Right(game);

    /// <returns>The piece on this space if found</returns>
    public BoardPiece? Piece(ChessGame game) => game.AllPieces.Values.FirstOrDefault(p => p.Space.Index == this.Index);

    /// <returns> whether or not an enemy piece can capture a (theoretical) piece that would be placed on the given space </returns>
    public bool IsThreateningSpace(ref List<Move> enemy) => enemy.FirstOrDefault(move => move.Destination == this) != null;
}
