using Android.Graphics;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;

namespace Chess.Dialogs;

public interface IProfileDialog : IMaterialDialog
{
    public MainActivity App { get; set; }
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; }
    public ShapeableImageView? DialogProfilePicture { get; set; }
    public Button? EditProfilePicture { get; set; }
    public TextView? ThemeText { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextView? EditProfileUsername { get; set; }
    public FloatingActionButton? EditUsername { get; set; }
    public UsernameDialog? UsernameDialog { get; set; }
    public Bitmap? PhotoBitmap { get; set; }
    public bool WasShown { get; set; }

    public void OnUsernameChange(string username);
    public void OnSelectPhoto(Bitmap photo);
}
