using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public class Capture(IPiece origin, IPiece destination) : ICapture
{
    /*[DataMember]*/ public ISpace Destination { get; set; } = destination.Space;
    /*[DataMember]*/ public int DestinationId { get; set; } = destination.Id;
    /*[DataMember]*/ public ISpace Origin { get; set; } = origin.Space;
    /*[DataMember]*/ public IPiece OriginPiece { get; set; } = origin;
    /*[DataMember]*/ public int OriginId { get; set; } = origin.Id;
    /*[DataMember]*/ public IPiece Piece { get; } = destination;
}
