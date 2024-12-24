namespace Chess.ChessBoard;

public class Move(ISpace origin, ISpace destination, bool capture = false, bool enPassantCapturable = false) : IMove
{
    public ISpace Destination { get; set; } = destination;
    public int DestinationId { get; set; } = destination.Id;
    public ISpace Origin { get; set; } = origin;
    public int OriginId { get; set; } = origin.Id;
    public bool Capture { get; set; } = capture;
    public bool EnPassantCapturable { get; set; } = enPassantCapturable;
    public override string ToString()
    {
        return $@"Move
{{
[{nameof(Capture)}: {Capture}],
[{nameof(Origin)}: {Origin}],
[{nameof(Destination)}: {Destination}],
}}";
    }
}


public interface IMove
{
    public ISpace Destination { get; set; }
    public int DestinationId { get; set; }
    public ISpace Origin { get; set; }
    public int OriginId { get; set; }
    public bool Capture { get; set; }
    public bool EnPassantCapturable { get; set; }
}

public interface ISpecial : IMove
{
    public bool Condition1 { get; set; }
    public bool Condition2 { get; set; }
    public void OnMove();
}