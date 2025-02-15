using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

[JsonDerivedType(typeof(Promote))]
[JsonDerivedType(typeof(IPromoteAndCapture))]
[JsonDerivedType(typeof(PromoteKnight))]
[JsonDerivedType(typeof(PromoteQueen))]
[JsonDerivedType(typeof(PromoteRook))]
[JsonDerivedType(typeof(PromoteBishop))]
[JsonPolymorphic(TypeDiscriminatorPropertyName = $"${nameof(IPromote)}",
    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
public interface IPromote : Pawn.ISpecialMove;
public interface INetworkedPromote : Pawn.INetworkedSpecialMove;
