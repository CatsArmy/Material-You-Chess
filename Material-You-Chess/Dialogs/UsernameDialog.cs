using Android.Content;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public class UsernameDialog : IUsernameDialog
{
    public AndroidX.AppCompat.App.AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public AppCompatActivity App { get; set; }
    public TextInputEditText UsernameInput { get; set; }
    public TextInputLayout UsernameLayout { get; set; }
    public Action<string> OnConfirmation { get; set; }

    public UsernameDialog(AppCompatActivity App, Action<string> OnConfirmation)
    {
        this.App = App;
        Builder = new MaterialAlertDialogBuilder(App, Resource.Style.ThemeOverlay_Catalog_MaterialAlertDialog_Centered_FullWidthButtons);
        Builder.SetTitle("Edit Username");
        Builder.SetView(Resource.Layout.username_dialog);
        Builder.SetPositiveButton("Confirm", OnConfirm);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
        this.OnConfirmation = OnConfirmation;
    }

    public void OnShow(object sender, EventArgs args)
    {
        UsernameInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.MainUsernameInput);
        UsernameLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.MainUsernameLayout);
        UsernameInput.Hint = FirebaseAuth.Instance.CurrentUser.DisplayName;
    }
    public void OnConfirm(object sender, DialogClickEventArgs args)
    {
        this.OnConfirmation(UsernameInput.Text);
    }
    public void OnCancel(object sender, DialogClickEventArgs args) { }
}
