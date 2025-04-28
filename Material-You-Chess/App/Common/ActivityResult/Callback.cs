using Android.Runtime;
using AndroidX.Activity.Result;

namespace Material.You.Chess.App.Common.ActivityResult;

public class ActivityResultCallback<O>(Action<O?> callback) : Java.Lang.Object, IActivityResultCallback where O : class, IJavaObject
{
    public bool IsAsync = false;
    private readonly Action<O?> callback = callback;
    public ActivityResultCallback(TaskCompletionSource<O?> tcs) : this(tcs.SetResult) => this.IsAsync = true;

    public void OnActivityResult(Java.Lang.Object? result) => this.OnActivityResult(result as O);
    public void OnActivityResult(O? result) => callback(result);
}
