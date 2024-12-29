using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public interface IMove
{
    /*[DataMember]*/ public ISpace Destination { get; set; }
    /*[DataMember]*/ public int DestinationId { get; set; }
    /*[DataMember]*/ public ISpace Origin { get; set; }
    /*[DataMember]*/ public IPiece OriginPiece { get; set; }
    /*[DataMember]*/ public int OriginId { get; set; }
    public virtual void Select()
    {
        this.Destination.SelectSpace();
        this.Origin.SelectSpace();
    }

    public virtual void Unselect()
    {
        this.Destination.UnselectSpace();
        this.Origin.UnselectSpace();
    }
}
