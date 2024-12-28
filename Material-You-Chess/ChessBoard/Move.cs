using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public class Move(IPiece origin, ISpace destination) : IMove
{
    [DataMember] public ISpace Destination { get; set; } = destination;
    [DataMember] public int DestinationId { get; set; } = destination.Id;
    [DataMember] public ISpace Origin { get; set; } = origin.Space;
    [DataMember] public IPiece OriginPiece { get; set; } = origin;
    [DataMember] public int OriginId { get; set; } = origin.Id;
}

public interface IMove
{
    [DataMember] public ISpace Destination { get; set; }
    [DataMember] public int DestinationId { get; set; }
    [DataMember] public ISpace Origin { get; set; }
    [DataMember] public IPiece OriginPiece { get; set; }
    [DataMember] public int OriginId { get; set; }
}

public class Capture(IPiece origin, IPiece destination) : ICapture
{
    [DataMember] public ISpace Destination { get; set; } = destination.Space;
    [DataMember] public int DestinationId { get; set; } = destination.Id;
    [DataMember] public ISpace Origin { get; set; } = origin.Space;
    [DataMember] public IPiece OriginPiece { get; set; } = origin;
    [DataMember] public int OriginId { get; set; } = origin.Id;
    [DataMember] public IPiece Piece { get; } = destination;
}

public interface ICapture : IMove
{
    [DataMember] public IPiece Piece { get; }
}