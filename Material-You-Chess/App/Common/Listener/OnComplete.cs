using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

public class OnComplete(Action<Android.Gms.Tasks.Task> action) : Java.Lang.Object, IOnCompleteListener
{
    void IOnCompleteListener.OnComplete(Android.Gms.Tasks.Task task) => action(task);
}