using System.Text.Json.Serialization;

namespace Chess.Game.Moves;

[JsonDerivedType(typeof(IPromoteAndCapture))]
[JsonDerivedType(typeof(EnPassant))]
[JsonDerivedType(typeof(Capture))]
[JsonPolymorphic(TypeDiscriminatorPropertyName = $"${nameof(ICapture)}",
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
public interface ICapture : IMove
{
    public IPiece Piece { get; }
}

public interface INetworkedCapture : INetworkedMove
{
    public (string, int) Piece { get; }
}