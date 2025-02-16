using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public interface IPromote : Pawn.ISpecialMove;
public interface INetworkedPromote : Pawn.INetworkedSpecialMove;
