using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public interface IPromotionDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public Pawn? Caller { get; set; }
    public Promotion? Move { get; set; }
    public List<int> Id { get; }
    public void Show(ChessGame game, Pawn pawn, Promotion Move);
    public void OnShow(object? sender, EventArgs args);
    public void OnConfirm(object? sender, EventArgs args);
}
