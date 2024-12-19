using Android.Content;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;

namespace Chess.Dialogs;

public class LogoutDialog : IMaterialDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    private Action OnConfirmation { get; set; }
    public LogoutDialog(AppCompatActivity App, Action OnConfirmation)
    {
        Builder = new MaterialAlertDialogBuilder(App, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetIcon(App.Resources.GetDrawable(Resource.Drawable.outline_settings_account_box, Builder.Context.Theme));
        Builder.SetTitle("Logout");
        Builder.SetMessage("Are you sure you want to logout?");
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Builder.SetNegativeButton("Cancel", OnCancel);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.OnConfirmation = OnConfirmation;
    }

    public void OnShow(object sender, EventArgs args) { }
    public void OnConfirm(object sender, DialogClickEventArgs args)
    {
        FirebaseAuth.Instance.SignOut();
        this.OnConfirmation();
    }
    public void OnCancel(object sender, DialogClickEventArgs args) { }
}