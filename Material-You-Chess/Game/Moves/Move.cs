using System.Text.Json.Serialization;
using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

[JsonPolymorphic()]
[JsonDerivedType(typeof(Move), nameof(Move))]
[JsonDerivedType(typeof(Capture), nameof(Capture))]
[JsonDerivedType(typeof(MoveOnly), nameof(MoveOnly))]
[JsonDerivedType(typeof(EnPassant), nameof(EnPassant))]
[JsonDerivedType(typeof(Promotion), nameof(Promotion))]
[JsonDerivedType(typeof(DoubleMove), nameof(DoubleMove))]
[JsonDerivedType(typeof(KingSideCastle), nameof(KingSideCastle))]
[JsonDerivedType(typeof(QueenSideCastle), nameof(QueenSideCastle))]
[JsonDerivedType(typeof(PromotionCapture), nameof(PromotionCapture))]
public class Move(BoardPiece origin, BoardSpace destination)
{
    /// <summary> The piece that will be moved </summary>
    public BoardPiece Origin { get; set; } = origin;

    /// <summary>The space the origin piece will be moved to </summary>
    public BoardSpace Destination { get; set; } = destination;

    /// <summary> 
    /// When Selecting a piece Shows the Visual Indicator of which spaces this move will pass through if played 
    /// </summary>
    public virtual void IndicateMoveable()
    {
        this.Destination.IndicateMoveable();
        this.Origin.Space.IndicateMoveable();
    }

    /// <summary> 
    /// When the selected piece is unselected Hides the Visual Indicator of which spaces this move will pass through if played 
    /// </summary>
    public virtual void UnindicateMoveable()
    {
        this.Destination.IndicateUnmovable();
        this.Origin.Space.IndicateUnmovable();
        this.Origin.LastSpace?.IndicateUnmovable();
    }

    /// <summary> 
    /// When the move is played Shows a Visual Indicator of which spaces the selected piece passed through 
    /// </summary>
    public virtual void Select()
    {
        this.Destination.Select();
        this.Origin.Space.Select();
    }

    /// <summary> 
    /// When the next move is played Hides the old Move Visual Indicator of which spaces the selected piece passed through 
    /// </summary>
    public virtual void Unselect()
    {
        this.Destination.Unselect();
        this.Origin.Space.Unselect();
        this.Origin.LastSpace?.Unselect();
    }
}
