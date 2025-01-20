using Chess.Game;

namespace Chess.Game.Moves;

public interface ICapture : IMove
{

    public IPiece Piece { get; }
}

public interface INetworkedCapture : INetworkedMove
{

    public (string, int) Piece { get; }
}