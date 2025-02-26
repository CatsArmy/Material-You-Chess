using Chess.Game.Board;

namespace Chess.Game.Player;

public interface IPlayer
{
    public string Name { get; set; }

    public GameOutcome Outcome { get; set; }

    public Dictionary<(string, int), BoardPiece> Pieces { get; }

    public List<Pawn> Pawns { get; }

    public Rook? Rook1 { get; set; }

    public Knight? Knight1 { get; set; }

    public Bishop? Bishop1 { get; set; }

    public King? King { get; set; }

    public Queen? Queen { get; set; }

    public Bishop? Bishop2 { get; set; }

    public Knight? Knight2 { get; set; }

    public Rook? Rook2 { get; set; }
}
