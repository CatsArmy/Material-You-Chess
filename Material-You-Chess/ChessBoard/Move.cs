namespace Chess.ChessBoard;

[Serializable]
public class Move
{
    [NonSerialized]
    public ISpace Space;
    internal int SpaceId;
    public bool Capture;
    public bool EnPassantCapturable = false;

    public Move(ISpace space, bool capture = false, bool enPassantCapturable = false)
    {
        this.Space = space;
        this.Capture = capture;
        this.EnPassantCapturable = enPassantCapturable;
        this.SpaceId = this.Space.Id;
    }
}
