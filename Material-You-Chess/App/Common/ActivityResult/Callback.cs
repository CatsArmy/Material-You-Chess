using AndroidX.Activity.Result;

namespace Chess.App.Common.ActivityResult;

public interface IActivityResultCallback<O> : IActivityResultCallback where O : Java.Lang.Object
{
    public void OnActivityResult(O? result);
}

public class ActivityResultCallback<O>(Action<O?> callback) : Java.Lang.Object, IActivityResultCallback<O> where O : Java.Lang.Object
{
    public bool IsAsync = false;

    public ActivityResultCallback(TaskCompletionSource<O?> tcs) : this(tcs.SetResult) => this.IsAsync = true;

    public void OnActivityResult(Java.Lang.Object? result) => this.OnActivityResult(result as O);

    public void OnActivityResult(O? result) => callback(result);
}