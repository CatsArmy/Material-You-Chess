using Chess.Game.Board;

namespace Chess.Game.Moves;

public class EnPassant(Pawn origin, BoardSpace destination, Pawn Piece) : Capture(origin, destination, Piece);
