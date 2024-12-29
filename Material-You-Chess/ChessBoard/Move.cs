using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public class Move(IPiece origin, ISpace destination) : IMove
{
    /*[DataMember]*/ public ISpace Destination { get; set; } = destination;
    /*[DataMember]*/ public int DestinationId { get; set; } = destination.Id;
    /*[DataMember]*/ public ISpace Origin { get; set; } = origin.Space;
    /*[DataMember]*/ public IPiece OriginPiece { get; set; } = origin;
    /*[DataMember]*/ public int OriginId { get; set; } = origin.Id;
}
