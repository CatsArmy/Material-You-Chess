using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public interface IPiece
{
    [DataMember]
    public ISpace Space { get; set; }

    [DataMember]
    public ImageView? Piece { get; set; }

    [DataMember]
    public int Id { get; }

    [DataMember]
    public bool IsWhite { get; }

    [DataMember]
    public (string, int) Index { get; }

    [DataMember]
    public char Abbreviation { get; set; }

    public void Update();
    public List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces);
    public void Move(IPiece destination) => this.Move(destination.Space);
    public void Move(ISpace destination);
    public void Capture(IPiece destination, Dictionary<(string, int), IPiece> pieces);
    public void Diagonals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void DiagonalsUpRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void DiagonalsUpLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void DiagonalsDownRight(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void DiagonalsDownLeft(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void Horizontals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isRight, ref List<IMove> moves);
    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, ref List<IMove> moves);
    public void Verticals(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces, bool isUp, ref List<IMove> moves);
    public (ISpace?, ISpace?) DiagonalMovesUp(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesDown(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesRight(Dictionary<(char, int), ISpace> board);
    public (ISpace?, ISpace?) DiagonalMovesLeft(Dictionary<(char, int), ISpace> board);
}
