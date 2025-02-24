using Chess.Game.Board;

namespace Chess.Game.Moves;

public interface IMove
{
    //public TypeString TypeString { get; }

    public BoardSpace Destination { get; set; }

    public int DestinationId { get; set; }

    public BoardSpace Origin { get; set; }

    public BoardPiece OriginPiece { get; set; }

    public int OriginId { get; set; }

    public void Select();

    public void Unselect();
}