using System.Text.Json.Serialization;
using Chess.Game.Board;
using Google.Android.Material.Badge;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public class Capture(BoardPiece origin, BoardSpace destination, BoardPiece Piece) : Move(origin, destination)
{
    public Capture(BoardPiece origin, BoardPiece destination) : this(origin, destination.Space, destination) { }

    /// <summary>
    /// The piece that will be captured by the origin piece
    /// </summary>
    public BoardPiece Piece { get; set; } = Piece;

    /// <summary> 
    /// When the Selecting a piece Shows the Visual Indicator of which spaces this move will pass through if played 
    /// </summary>
    public override void IndicateMoveable()
    {
        base.IndicateMoveable();
        this.Piece.Space.IndicateMoveable();
        BadgeUtils.AttachBadgeDrawable(ChessGame.CaptureAlertTopRightBadge!, this.Piece.PieceView!);
        BadgeUtils.AttachBadgeDrawable(ChessGame.CaptureAlertTopLeftBadge!, this.Piece.PieceView!);
        BadgeUtils.AttachBadgeDrawable(ChessGame.CaptureAlertBottomRightBadge!, this.Piece.PieceView!);
        BadgeUtils.AttachBadgeDrawable(ChessGame.CaptureAlertBottomLeftBadge!, this.Piece.PieceView!);
    }

    /// <summary> 
    /// When the selected piece is unselected Hides the Visual Indicator of which spaces this move will pass through if played 
    /// </summary>
    public override void UnindicateMoveable()
    {
        base.UnindicateMoveable();
        this.Piece.Space.IndicateUnmovable();
        BadgeUtils.DetachBadgeDrawable(ChessGame.CaptureAlertTopRightBadge, this.Piece.PieceView!);
        BadgeUtils.DetachBadgeDrawable(ChessGame.CaptureAlertTopLeftBadge, this.Piece.PieceView!);
        BadgeUtils.DetachBadgeDrawable(ChessGame.CaptureAlertBottomRightBadge, this.Piece.PieceView!);
        BadgeUtils.DetachBadgeDrawable(ChessGame.CaptureAlertBottomLeftBadge, this.Piece.PieceView!);
    }

    /// <summary> 
    /// When the move is played Shows a Visual Indicator of which piece and spaces the selected piece passed through 
    /// </summary>
    public override void Select()
    {
        base.Select();
        this.Piece.Space.Select();
    }

    /// <summary> 
    /// When the next move is played Hides the old Move Visual Indicator of which piece and spaces the selected piece passed through 
    /// </summary>
    public override void Unselect()
    {
        base.Unselect();
        this.Piece.Space.Unselect();
    }
}
