using System.Text.Json.Serialization;

namespace Chess.Game.Board;

public class BoardSpace(ImageView? Space, char file, int rank, bool IsWhite, int Id) : ISpace
{
    [JsonIgnore] public ImageView? Space { get; } = Space;

    public bool IsWhite { get; } = IsWhite;

    public int Id { get; } = Id;

    public (char, int) Index { get; } = (file, rank);

    public char File { get; } = file;

    public int Rank { get; } = rank;

    private const int select = 1;
    public virtual void Select() => this.Space?.SetImageLevel(select);

    private const int unselect = 0;
    public virtual void Unselect() => this.Space?.SetImageLevel(unselect);

    public override string ToString() => $"{File}{Rank}";

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
