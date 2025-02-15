using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[JsonPolymorphic(TypeDiscriminatorPropertyName = $"${nameof(IMove)}", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(Move))]
// [JsonDerivedType(typeof(INetworkedMove))]
[JsonDerivedType(typeof(ICapture))]
[JsonDerivedType(typeof(Pawn.ISpecialMove))]
public interface IMove
{
    public ISpace Destination { get; set; }

    public int DestinationId { get; set; }

    public ISpace Origin { get; set; }

    public IPiece OriginPiece { get; set; }

    public int OriginId { get; set; }

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


public interface INetworkedMove
{
    public (char, int) Destination { get; set; }

    public (char, int) Origin { get; set; }

    public (string, int) OriginPiece { get; set; }

    public IMove FromNetworked(IChessGame game);
}