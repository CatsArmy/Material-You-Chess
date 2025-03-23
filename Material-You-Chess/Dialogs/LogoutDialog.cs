using Android.Content;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public class LogoutDialog : IMaterialDialog
{
    public AlertDialog Dialog { get; set; }
    public AlertDialog.Builder Builder { get; set; }
    public Action OnLogout { get; set; }

    public LogoutDialog(Action OnLogout, Context context)
    {
        this.Builder = new MaterialAlertDialogBuilder(context)!.SetTitle("Logout")!.SetIcon(Resource.Drawable.logout)
            !.SetPositiveButton("Confirm", this.OnConfirm)!.SetMessage("Are you sure you want to logout?")!;

        this.Dialog = this.Builder.Create();
        this.OnLogout = OnLogout;
    }

    public void Show(object? sender, EventArgs args) => this.Dialog?.Show();

    public void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        FirebaseAuth.Instance.SignOut();
        this.OnLogout();
    }
}
