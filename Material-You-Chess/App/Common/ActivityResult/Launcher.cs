using AndroidX.Activity.Result;

namespace Chess.App.Common.ActivityResult;

public class ActivityResultLauncher<I>(ActivityResultLauncher Launcher) where I : Java.Lang.Object
{
    public void Launch(I? input) => Launcher.Launch(input);
}