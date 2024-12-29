using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public class BoardSpace(ImageView? Space, char file, int rank, bool IsWhite, int Id) : ISpace
{
    /*[IgnoreDataMember]*/
    public ImageView? Space { get; } = Space;

    /*[DataMember]*/
    public bool IsWhite { get; } = IsWhite;

    /*[DataMember]*/
    public int Id { get; } = Id;

    /*[DataMember]*/
    public (char, int) Index { get; } = (file, rank);

    /*[DataMember]*/
    public char File { get; } = file;

    /*[DataMember]*/
    public int Rank { get; } = rank;

    public override string ToString() => $"{File}{Rank}";

    public void SelectSpace() => this.Space?.SetImageLevel(ISpace.Select);

    public void UnselectSpace() => this.Space?.SetImageLevel(ISpace.Unselect);

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

    public IPiece? Piece(Dictionary<(string, int), IPiece> boardPieces) => boardPieces.Values.FirstOrDefault(p => p.Space.Index == this.Index);
}
