using System.Text.Json.Serialization;

namespace Chess.Game.Moves;

[JsonDerivedType(typeof(PromoteAndCapture))]
[JsonDerivedType(typeof(PromoteBishopAndCapture))]
[JsonDerivedType(typeof(PromoteKnightAndCapture))]
[JsonDerivedType(typeof(PromoteRookAndCapture))]
[JsonDerivedType(typeof(PromoteQueenAndCapture))]
[JsonPolymorphic(TypeDiscriminatorPropertyName = $"${nameof(IPromoteAndCapture)}",
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
public interface IPromoteAndCapture : IPromote, ICapture;
public interface INetworkedPromoteAndCapture : INetworkedPromote, INetworkedCapture;
