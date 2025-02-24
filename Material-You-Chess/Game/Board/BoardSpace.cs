using System.Text.Json.Serialization;

namespace Chess.Game.Board;

public class BoardSpace : ISpace
{
    [JsonIgnore] public ImageView? SpaceView { get; internal set; }
    [JsonIgnore] public (char, int) Index => (this.File, this.Rank);
    [JsonIgnore] public int Id { get; protected set; }
    public bool IsWhite { get; protected set; }
    public char File { get; protected set; }
    public int Rank { get; protected set; }

    private const int select = 1;
    public virtual void Select() => this.SpaceView?.SetImageLevel(select);
    public virtual void Unselect() => this.SpaceView?.SetImageLevel(unselect);
    private const int unselect = 0;


    public BoardSpace(ImageView? Space, char File, int Rank, bool IsWhite, int Id)
    {
        this.Id = Id;
        this.SpaceView = Space;
        this.IsWhite = IsWhite;
        this.File = File;
        this.Rank = Rank;
    }

    public BoardSpace(ChessGame game, bool IsWhite, char File, int Rank, int Id)
    {
        this.Id = Id;
        this.SpaceView = game.Activity.BoardLayout!.FindViewById<ImageView>(Id);
        this.IsWhite = IsWhite;
        this.File = File;
        this.Rank = Rank;
    }

    public BoardSpace? DiagonalUp(Dictionary<(char, int), BoardSpace> board, bool isRight)
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

    public BoardSpace? DiagonalDown(Dictionary<(char, int), BoardSpace> board, bool isRight)
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

    public BoardSpace? Up(Dictionary<(char, int), BoardSpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, ++Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Down(Dictionary<(char, int), BoardSpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, --Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Right(Dictionary<(char, int), BoardSpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((++File, Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Left(Dictionary<(char, int), BoardSpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((--File, Rank), out var value))
            return null;

        return value;
    }

    public BoardSpace? Forward(Dictionary<(char, int), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Down(board);
        return this.Up(board);
    }

    public BoardSpace? Backward(Dictionary<(char, int), BoardSpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Up(board);
        return this.Down(board);
    }

    public BoardPiece? Piece(Dictionary<(string, int), BoardPiece> boardPieces)
        => boardPieces.Values.FirstOrDefault(p => p.Space.Index == this.Index);
}
