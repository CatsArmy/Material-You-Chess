using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;
namespace Chess.Game;

public interface IChessGame
{
    public Dictionary<(string Prefix, int Count), BoardPiece> AllPieces { get; }

    public Dictionary<(char file, int rank), BoardSpace> Board { get; }

    public List<Move>? Moves { get; set; }

    public White? Player1 { get; set; }

    public Black? Player2 { get; set; }

    public Move? LastMove { get; set; }

    public BoardPiece? Selected { get; set; }
}
