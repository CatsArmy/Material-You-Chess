using Chess.Game.Moves;
using Chess.Game.Player;

namespace Chess.Game;

//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip

public interface IChessGame
{
    public Dictionary<(string, int), IPiece> AllPieces { get; }

    public Dictionary<(char, int), ISpace> Board { get; }

    public List<IMove>? Moves { get; set; }

    public IPlayer? White { get; set; }

    public IPlayer? Black { get; set; }

    public IMove? LastMove { get; set; }

    public IPiece? Selected { get; set; }
}
