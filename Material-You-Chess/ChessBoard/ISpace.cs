using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public interface ISpace
{
    /*[IgnoreDataMember]*/ public ImageView? Space { get; }
    /*[DataMember]*/ public bool IsWhite { get; }
    /*[DataMember]*/ public int Id { get; }
    /*[DataMember]*/ public (char, int) Index { get; }
    /*[DataMember]*/ public char File { get; }
    /*[DataMember]*/ public int Rank { get; }

    /*[IgnoreDataMember]*/ public const int Select = 1;
    /*[IgnoreDataMember]*/ public const int Unselect = 0;

    public void SelectSpace();
    public void UnselectSpace();
    public ISpace? DiagonalUp(Dictionary<(char, int), ISpace> board, bool isRight);
    public ISpace? DiagonalDown(Dictionary<(char, int), ISpace> board, bool isRight);
    public ISpace? Up(Dictionary<(char, int), ISpace> board);
    public ISpace? Down(Dictionary<(char, int), ISpace> board);
    public ISpace? Right(Dictionary<(char, int), ISpace> board);
    public ISpace? Left(Dictionary<(char, int), ISpace> board);
    public ISpace? Forward(Dictionary<(char, int), ISpace> board, bool isWhite);
    public ISpace? Backward(Dictionary<(char, int), ISpace> board, bool isWhite);
    public IPiece? Piece(Dictionary<(string, int), IPiece> boardPieces);
}
