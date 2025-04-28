using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

/// <summary>
/// A Move that can never capture a enemy piece unlike other Move types.
/// this is used to make sure that a space is correctly marked as Threatened
/// </summary>
public class MoveOnly(BoardPiece origin, BoardSpace destination) : Move(origin, destination) { }
