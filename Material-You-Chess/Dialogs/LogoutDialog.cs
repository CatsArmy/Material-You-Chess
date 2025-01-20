using Android.Content;
using Chess.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public class LogoutDialog : IMaterialDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public MainActivity App { get; set; }
    public LogoutDialog(MainActivity App)
    {
        this.App = App;
        this.Builder = new MaterialAlertDialogBuilder(App);
        this.Builder.SetIcon(Resource.Drawable.outline_person_remove);
        this.Builder.SetTitle("Logout");
        this.Builder.SetMessage("Are you sure you want to logout?");
        this.Builder.SetPositiveButton("Confirm", OnConfirm);
        this.Builder.SetNegativeButton("Cancel", OnCancel);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += OnShow;
    }

    public void OnShow(object? sender, EventArgs args) { }

    public void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        FirebaseAuth.Instance.SignOut();
        this.App.UpdateUserState();
    }

    public void OnCancel(object? sender, DialogClickEventArgs args) { }
}