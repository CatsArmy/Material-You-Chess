using Chess.Game;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public interface IPromotionDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public IPromote? Move { get; set; }

    public void Show(ChessGame game, IPromote Move);
    public void OnShow(object? sender, EventArgs args);
    public void OnConfirm(object? sender, EventArgs args);
}
