using AndroidX.Activity.Result;

namespace Chess.Util;

public class ActivityResultCallback<O> : Java.Lang.Object, IActivityResultCallback where O : Java.Lang.Object
{
    private readonly Action<O> _callback;
    public ActivityResultCallback(Action<O> callback) => _callback = callback;
    public ActivityResultCallback(TaskCompletionSource<O> tcs) => _callback = tcs.SetResult;
    public void OnActivityResult(Java.Lang.Object? result) => _callback((O)result!);
}
