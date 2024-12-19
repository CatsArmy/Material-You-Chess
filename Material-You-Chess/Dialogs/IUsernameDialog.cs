using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public interface IUsernameDialog : IMaterialDialog
{
    public TextInputEditText UsernameInput { get; set; }
    public TextInputLayout UsernameLayout { get; set; }
    public Action<string> OnConfirmation { get; set; }
}
