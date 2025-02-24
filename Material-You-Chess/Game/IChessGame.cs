using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;

namespace Chess.Game;

//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip

public interface IChessGame
{
    public Dictionary<(string, int), BoardPiece> AllPieces { get; }

    public Dictionary<(char, int), BoardSpace> Board { get; }

    public List<Move>? Moves { get; set; }

    public IPlayer? White { get; set; }

    public IPlayer? Black { get; set; }

    public Move? LastMove { get; set; }

    public BoardPiece? Selected { get; set; }
}
