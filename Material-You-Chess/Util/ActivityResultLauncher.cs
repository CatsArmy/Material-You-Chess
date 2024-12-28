using AndroidX.Activity.Result;

namespace Chess.Util;

public abstract class ActivityResultLauncher<I> : ActivityResultLauncher where I : Java.Lang.Object
{
    public void Launch(I? input)
    {
        base.Launch(input);
    }
}