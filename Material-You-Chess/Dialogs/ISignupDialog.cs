using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public interface ISignupDialog : ILoginDialog
{
    public string Username { get; set; }
    public TextInputEditText UsernameInput { get; set; }
    public TextInputLayout UsernameLayout { get; set; }
}
