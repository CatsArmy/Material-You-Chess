using Android.Content;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public interface IMaterialDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public void OnShow(object? sender, EventArgs args);
    public void OnConfirm(object? sender, DialogClickEventArgs args);
    public void OnCancel(object? sender, DialogClickEventArgs args);
}
