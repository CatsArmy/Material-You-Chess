using System.Runtime.Serialization;

namespace Chess.Game;

public interface ISpace
{
     public ImageView? Space { get; }
     public bool IsWhite { get; }
     public int Id { get; }
     public (char, int) Index { get; }
     public char File { get; }
     public int Rank { get; }

     public const int Select = 1;
     public const int Unselect = 0;

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
