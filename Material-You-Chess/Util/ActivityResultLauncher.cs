using AndroidX.Activity.Result;

namespace Chess.Util;

public class ActivityResultLauncher<I>(ActivityResultLauncher Launcher) where I : Java.Lang.Object
{
    public void Launch(I? input) => Launcher.Launch(input);
}