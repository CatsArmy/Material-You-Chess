using System;

namespace Chess.ChessBoard;

[Serializable]
public class Move
{
    [NonSerialized]
    public Space Space;
    internal int SpaceId;
    public bool Capture;
    public bool EnPassantCapturable = false;

    public Move(Space space, bool capture = false, bool enPassantCapturable = false)
    {
        this.Space = space;
        this.Capture = capture;
        this.EnPassantCapturable = enPassantCapturable;
        this.SpaceId = this.Space.spaceId;
    }
}
