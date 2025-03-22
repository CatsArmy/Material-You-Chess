using Android.Runtime;
using AndroidX.Activity.Result;

namespace Chess.App.Common.ActivityResult;

public interface IActivityResultCallback<O> : IActivityResultCallback where O : class, IJavaObject
{
    public void OnActivityResult(O? result);
}

public class ActivityResultCallback<O>(Action<O?> callback) : Java.Lang.Object, IActivityResultCallback<O> where O : class, IJavaObject
{
    private readonly Action<O?> callback = callback;
    public bool IsAsync = false;

    public ActivityResultCallback(TaskCompletionSource<O?> tcs) : this(tcs.SetResult) => this.IsAsync = true;

    public void OnActivityResult(Java.Lang.Object? result) => this.OnActivityResult(result as O);

    public void OnActivityResult(O? result) => callback(result);
}
