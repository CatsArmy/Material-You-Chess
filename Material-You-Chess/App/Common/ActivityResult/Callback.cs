using Android.Runtime;
using AndroidX.Activity.Result;
using AndroidX.Credentials;

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

#region

internal sealed class CredentialManagerCallback<TResult, TException>(CancellationToken cancellationToken)
    : AsyncCallback<TResult, TException>(cancellationToken)
    , ICredentialManagerCallback
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    public void OnResult(Java.Lang.Object? result)
    {
        var parsedResult = result is not null
            ? (TResult)result
            : null;

        ReportSuccess(parsedResult);
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}

internal class CredentialManagerCallback<TException>(CancellationToken cancellationToken)
    : AsyncCallback<TException>(cancellationToken)
    , ICredentialManagerCallback
    where TException : Java.Lang.Exception
{
    public void OnResult(Java.Lang.Object? result)
    {
        ReportSuccess();
    }

    public void OnError(Java.Lang.Object e)
    {
        var exception = e.JavaCast<TException>();
        ReportException(exception);
    }
}

internal abstract class AsyncCallback<TResult, TException> : Java.Lang.Object
    where TResult : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource<TResult?> _taskCompletionSource;

    public AsyncCallback(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource<TResult?>();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task<TResult?> Task => _taskCompletionSource.Task;

    protected void ReportSuccess(TResult? result)
    {
        _taskCompletionSource.TrySetResult(result);
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}

internal abstract class AsyncCallback<TException> : Java.Lang.Object
    where TException : Java.Lang.Exception
{
    private readonly TaskCompletionSource _taskCompletionSource;

    public AsyncCallback(CancellationToken cancellationToken)
    {
        _taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() => _taskCompletionSource.TrySetCanceled());
    }

    public Task Task => _taskCompletionSource.Task;

    protected void ReportSuccess()
    {
        _taskCompletionSource.TrySetResult();
    }

    protected void ReportException(TException exception)
    {
        _taskCompletionSource.TrySetException(exception);
    }
}
#endregion