using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Common;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public class Promotion(BoardPiece origin, BoardSpace destination, SerializedType? PromoteTo = null) : Move(origin, destination)
{
    public SerializedType? PromoteTo { get; set; } = PromoteTo;

    public Promotion(BoardPiece origin, BoardSpace destination, Type typeTo) : this(origin, destination, PromoteTo: new(typeTo)) { }
}
