using System.Text.Json.Serialization;
using Material.You.Chess.Game.Board;
using Material.You.Chess.Game.Common;

namespace Material.You.Chess.Game.Moves;

[method: JsonConstructor]
public class Promotion(BoardPiece origin, BoardSpace destination, SerializedType? PromoteTo = null) : Move(origin, destination)
{
    /// <summary>
    /// The type the pawn promotes to. 
    /// this field is also used to indicate whether this move is ready to be sent
    /// </summary>
    public SerializedType? PromoteTo { get; set; } = PromoteTo;

    public Promotion(BoardPiece origin, BoardSpace destination, Type typeTo) : this(origin, destination, PromoteTo: new(typeTo)) { }
}
