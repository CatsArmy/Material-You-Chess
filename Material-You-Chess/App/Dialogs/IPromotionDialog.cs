using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.App.Dialogs;

public interface IPromotionDialog : IMaterialDialog
{
    public Pawn? Caller { get; set; }
    public Promotion? Move { get; set; }
    public List<int> IDs { get; }
    public void Show(ChessGame game, Pawn pawn, Promotion Move);
    public void OnConfirm(object? sender, EventArgs args);
}
