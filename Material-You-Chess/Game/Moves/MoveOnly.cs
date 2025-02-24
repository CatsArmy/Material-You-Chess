using Chess.Game.Board;

namespace Chess.Game.Moves;

//[JsonDerivedType(typeof(KingSideCastle), nameof(KingSideCastle))]
//[JsonDerivedType(typeof(QueenSideCastle), nameof(QueenSideCastle))]
//[JsonDerivedType(typeof(DoubleMove), nameof(DoubleMove))]
public class MoveOnly(BoardPiece origin, BoardSpace destination) : Move(origin, destination);
