using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

#pragma warning disable XAOBS001 // Type or member is obsolete

public class OnCanceled(Action action) : Java.Lang.Object, IOnCanceledListener
{
    void IOnCanceledListener.OnCanceled() => action();
}

#pragma warning restore XAOBS001 // Type or member is obsolete