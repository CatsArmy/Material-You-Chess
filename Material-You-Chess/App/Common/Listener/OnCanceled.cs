using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

public class OnCanceled(Action action) : Java.Lang.Object, IOnCanceledListener
{
    void IOnCanceledListener.OnCanceled() => action();
}