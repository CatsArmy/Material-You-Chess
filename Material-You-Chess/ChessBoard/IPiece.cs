namespace Chess.ChessBoard;

public interface IPiece
{
    public ImageView? Piece { get; set; }
    public int Id { get; set; }
    public bool IsWhite { get; set; }
    public (string, int) Index
    {
        get
        {
            string? resourceName = ChessActivity.Instance?.Resources?.GetResourceName(this.Space.Id);
            string? piece = resourceName?.Split("__")[1];
            return (piece[0..^1], int.Parse($"{piece[^1]}"));
        }
    }
    public ISpace Space { get; set; }
    public void Update() { }
    public List<Move> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces);
    public void Move(ISpace destination);
    public (int, ImageView?) FakeMove(int spaceId, ImageView? spaceView);
    public void Capture(IPiece destination, Dictionary<(string, int), IPiece> pieces);
    public void Diagonals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void DiagonalsUpRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void DiagonalsUpLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void DiagonalsDownRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void DiagonalsDownLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isRight, ref List<Move> moves);
    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<Move> moves);
    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isUp, ref List<Move> moves);
    public (ISpace?, ISpace?) DiagonalMovesUp(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesDown(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesRight(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesLeft(Dictionary<(char, int), ISpace> board);
}
