namespace Chess.ChessBoard;

public class Move
{
    public Move(Space space, bool capture = false, bool enPassantCapturable = false)
    {
        Space = space;
        Capture = capture;
        EnPassantCapturable = enPassantCapturable;
    }
    public Space Space;
    public bool Capture;
    public bool EnPassantCapturable = false;
}
