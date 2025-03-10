using AndroidX.Activity.Result;

namespace Chess.App.Common.ActivityResult;

public abstract class ActivityResultLauncher<I> : ActivityResultLauncher where I : Java.Lang.Object
{
    public void Launch(I? input) => base.Launch(input as Java.Lang.Object);
}
