namespace Chess.ChessBoard;

public class BoardSpace(ImageView? Space, bool IsWhite, int Id) : ISpace
{
    private const int Select = 1;
    private const int Unselect = 0;

    public ImageView? Space { get; set; } = Space;
    public bool IsWhite { get; set; } = IsWhite;
    public int Id { get; set; } = Id;
    public (char, int) Index { get => (this.File, this.Rank); }

    public char File { get => ChessActivity.Instance!.Resources!.GetResourceName(this.Id)!.Split("__")[1][0]!; }

    public int Rank { get => int.Parse($"{ChessActivity.Instance!.Resources!.GetResourceName(this.Id)!.Split("__")[1][^1]}")!; }

    public override string? ToString() => ChessActivity.Instance?.Resources?.GetResourceName(this.Id)?.Split("__")[1];

    public void SelectSpace() => this.Space?.SetImageLevel(Select);

    public void UnselectSpace() => this.Space?.SetImageLevel(Unselect);

    public ISpace? GetBoardSpace(Dictionary<(char, int), ISpace> board) => board[this.Index];

    public ISpace? DiagonalUp(Dictionary<(char, int), ISpace> board, bool isRight)
    {
        ISpace? up = this.Up(board);
        if (up == null)
            return null;

        if (isRight)
        {
            ISpace? right = up.Right(board);
            if (right == null)
                return null;

            return right;
        }

        ISpace? left = up.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public ISpace? DiagonalDown(Dictionary<(char, int), ISpace> board, bool isRight)
    {
        ISpace? down = Down(board);
        if (down == null)
            return null;

        if (isRight)
        {
            ISpace? right = down.Right(board);
            if (right == null)
                return null;

            return right;
        }

        ISpace? left = down.Left(board);
        if (left == null)
            return null;

        return left;
    }

    public ISpace? Up(Dictionary<(char, int), ISpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, ++Rank), out ISpace? value))
            return null;

        return value;
    }

    public ISpace? Down(Dictionary<(char, int), ISpace> board)
    {
        var Rank = this.Rank;
        if (!board.TryGetValue((File, --Rank), out ISpace? value))
            return null;

        return value;
    }

    public ISpace? Right(Dictionary<(char, int), ISpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((++File, Rank), out ISpace? value))
            return null;

        return value;
    }

    public ISpace? Left(Dictionary<(char, int), ISpace> board)
    {
        var File = this.File;
        if (!board.TryGetValue((--File, Rank), out ISpace? value))
            return null;

        return value;
    }

    public ISpace? Forward(Dictionary<(char, int), ISpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Down(board);
        return this.Up(board);
    }

    public ISpace? Backward(Dictionary<(char, int), ISpace> board, bool isWhite)
    {
        if (!isWhite)
            return this.Up(board);
        return this.Down(board);
    }

    public IPiece? Piece(Dictionary<(string, int), IPiece> boardPieces)
    {
        if (boardPieces.Values.Where(p => p.Space == this).ToList().Count <= 0)
            return null;

        return boardPieces.Values.FirstOrDefault(p => p.Space == this);
    }
}
