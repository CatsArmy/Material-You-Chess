using Chess.Game.Board;

namespace Chess.Game.Moves;
public class PromotionCapture(Pawn origin, BoardPiece destination, SerializedType? promoteTo = null) : Promotion(origin, destination.Space, promoteTo)
{
    public PromotionCapture(Pawn origin, BoardPiece destination, Type promoteTo) : this(origin, destination, new SerializedType(promoteTo)) { }
    public BoardPiece Piece { get; internal set; } = destination;
}
