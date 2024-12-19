﻿using AndroidX.AppCompat.App;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.MaterialSwitch;

namespace Chess.Dialogs;

public interface IProfileDialog : IMaterialDialog
{
    public AppCompatActivity App { get; set; }
    public Action UpdateInformation { get; set; }
    public Action<object, EventArgs> OpenPhotoPicker { get; set; }
    public UserProfileChangeRequest.Builder UserProfileChangeRequest { get; set; }
    public ShapeableImageView? DialogProfilePicture { get; set; }
    public Button? EditProfilePicture { get; set; }
    public TextView? ThemeText { get; set; }
    public MaterialSwitch? ThemeToggle { get; set; }
    public TextView? EditProfileUsername { get; set; }
    public FloatingActionButton? EditUsername { get; set; }
    public UsernameDialog? UsernameDialog { get; set; }
    public bool WasShown { get; set; }
    public void OnUsernameChange(string username);
    public void OnSelectPhoto(Android.Net.Uri photoUri);
}
