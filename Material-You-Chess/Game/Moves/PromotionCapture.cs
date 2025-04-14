using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Common;

namespace Chess.Game.Moves;

/// <param name="piece">destination</param>
[method: JsonConstructor]
public class PromotionCapture(BoardPiece origin, BoardPiece piece, SerializedType? PromoteTo = null) : Promotion(origin, piece.Space, PromoteTo)
{
    public PromotionCapture(Pawn origin, BoardPiece destination, Type typeTo) : this(origin, destination, PromoteTo: new(typeTo)) { }
    public BoardPiece Piece { get; internal set; } = piece;
}
