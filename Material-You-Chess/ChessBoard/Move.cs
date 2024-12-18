namespace Chess.ChessBoard;

[Serializable]
public class Move
{
    [NonSerialized]
    public BoardSpace Space;
    internal int SpaceId;
    public bool Capture;
    public bool EnPassantCapturable = false;

    public Move(BoardSpace space, bool capture = false, bool enPassantCapturable = false)
    {
        this.Space = space;
        this.Capture = capture;
        this.EnPassantCapturable = enPassantCapturable;
        this.SpaceId = this.Space.spaceId;
    }
}
