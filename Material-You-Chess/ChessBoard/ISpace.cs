namespace Chess.ChessBoard;

public interface ISpace
{
    public ImageView? SpaceView { get; set; }
    public bool IsWhite { get; set; }
    public int Id { get; set; }
    public (char, int) Index { get; }
    public char File { get; }
    public int Rank { get; }
    public void SelectSpace();
    public void UnselectSpace();
    public ISpace? GetBoardSpace(Dictionary<(char, int), ISpace> board);
    public ISpace? Forward(Dictionary<(char, int), ISpace> board, bool isWhite);
    public ISpace? Backward(Dictionary<(char, int), ISpace> board, bool isWhite);
    public ISpace? DiagonalUp(Dictionary<(char, int), ISpace> board, bool isRight);
    public ISpace? DiagonalDown(Dictionary<(char, int), ISpace> board, bool isRight);
    public ISpace? Up(Dictionary<(char, int), ISpace> board);
    public ISpace? Down(Dictionary<(char, int), ISpace> board);
    public ISpace? Right(Dictionary<(char, int), ISpace> board);
    public ISpace? Left(Dictionary<(char, int), ISpace> board);
    public IPiece? Piece(Dictionary<(string, int), IPiece> boardPieces);
}
