using Google.Android.Material.Floatingtoolbar;
using Material.You.Chess.Game.Board;
using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Player;

public interface IPlayer
{
    public string Name { get; }
    public bool IsCheckmated { get; set; }
    public Promotion? Promotion { get; set; }
    public FloatingToolbarLayout QuickPromotionAction { get; init; }
    public Dictionary<(string Prefix, int Count), BoardPiece> Pieces { get; }
    public List<Pawn> Pawns { get; }
    public Rook? Rook1 { get; set; }
    public Knight? Knight1 { get; set; }
    public Bishop? Bishop1 { get; set; }
    public King? King { get; set; }
    public Queen? Queen { get; set; }
    public Bishop? Bishop2 { get; set; }
    public Knight? Knight2 { get; set; }
    public Rook? Rook2 { get; set; }
    public BoardPiece? Selected { get; set; }
    public List<Move>? Moves { get; set; }
    public Move? LastMove { get; set; }
}
