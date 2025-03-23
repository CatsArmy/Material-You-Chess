
using Android.Gms.Tasks;

namespace Chess.App.Common;


#pragma warning disable XAOBS001 // Type or member is obsolete

public class OnCompleteListener(Action<Android.Gms.Tasks.Task> action) : Java.Lang.Object, IOnCompleteListener
{
    public void OnComplete(Android.Gms.Tasks.Task task) => action(task);
}

public class OnCanceledListener(Action action) : Java.Lang.Object, IOnCanceledListener
{
    public void OnCanceled() => action();
}

public class OnSuccessListener(Action<Java.Lang.Object?> action) : Java.Lang.Object, IOnSuccessListener
{
    public void OnSuccess(Java.Lang.Object? result) => action(result);
}

public class OnFailureListener(Action<Java.Lang.Exception> action) : Java.Lang.Object, IOnFailureListener
{
    public void OnFailure(Java.Lang.Exception result) => action(result);
}

#pragma warning restore XAOBS001 // Type or member is obsolete