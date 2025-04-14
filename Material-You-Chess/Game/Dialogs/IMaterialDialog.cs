using Android.Content;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Game.Dialogs;

/// <summary> Template Interface for dialogs with a custom view </summary>
public interface IMaterialDialog
{
    public AlertDialog Dialog { get; set; }
    public AlertDialog.Builder Builder { get; set; }

    public virtual void Show(object? sender, EventArgs args) { }
    public virtual void OnShow(object? sender, EventArgs args) { }
    public virtual void OnConfirm(object? sender, DialogClickEventArgs args) { }
    public virtual void OnCancel(object? sender, DialogClickEventArgs args) { }
}
