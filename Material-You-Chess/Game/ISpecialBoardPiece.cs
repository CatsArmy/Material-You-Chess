namespace Chess.Game;

public interface ISpecialBoardPiece : IPiece
{
    public bool HasMoved { get; set; }
}
