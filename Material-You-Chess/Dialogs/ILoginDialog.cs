using Google.Android.Material.TextField;

namespace Chess.Dialogs;

public interface ILoginDialog : IMaterialDialog
{
    public bool WasShown { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public TextInputEditText? EmailInput { get; set; }
    public TextInputLayout? EmailLayout { get; set; }
    public TextInputLayout? PasswordLayout { get; set; }
    public TextInputEditText? PasswordInput { get; set; }
    public Action OnSuccess { get; set; }

}
