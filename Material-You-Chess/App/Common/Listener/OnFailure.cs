using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

public class OnFailure(Action<Java.Lang.Exception> action) : Java.Lang.Object, IOnFailureListener
{
    void IOnFailureListener.OnFailure(Java.Lang.Exception result) => action(result);
}