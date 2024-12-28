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
    public TextInputEditText? UsernameInput { get; set; }
    public TextInputLayout? UsernameLayout { get; set; }
    public Action<string> OnConfirmation { get; set; }

    public UsernameDialog(AppCompatActivity App, Action<string> OnConfirmation)
    {
        this.App = App;
        this.Builder = new MaterialAlertDialogBuilder(App);
        this.Builder.SetTitle("Edit Username");
        this.Builder.SetView(Resource.Layout.username_dialog);
        this.Builder.SetPositiveButton("Confirm", OnConfirm);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += OnShow;
        this.OnConfirmation = OnConfirmation;
    }

    public void OnShow(object? sender, EventArgs args)
    {
        this.UsernameInput = this.Dialog.FindViewById<TextInputEditText>(Resource.Id.MainUsernameInput);
        this.UsernameLayout = this.Dialog.FindViewById<TextInputLayout>(Resource.Id.MainUsernameLayout);
        this.UsernameInput!.Hint = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
    }

    public void OnConfirm(object? sender, DialogClickEventArgs args)
    {
        this.OnConfirmation(this.UsernameInput!.Text!);
    }

    public void OnCancel(object? sender, DialogClickEventArgs args) { }
}
