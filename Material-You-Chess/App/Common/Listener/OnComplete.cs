using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

#pragma warning disable XAOBS001 // Type or member is obsolete

public class OnComplete(Action<Android.Gms.Tasks.Task> action) : Java.Lang.Object, IOnCompleteListener
{
    void IOnCompleteListener.OnComplete(Android.Gms.Tasks.Task task) => action(task);
}

#pragma warning restore XAOBS001 // Type or member is obsolete