using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

#pragma warning disable XAOBS001 // Type or member is obsolete

public class OnFailure(Action<Java.Lang.Exception> action) : Java.Lang.Object, IOnFailureListener
{
    void IOnFailureListener.OnFailure(Java.Lang.Exception result) => action(result);
}

#pragma warning restore XAOBS001 // Type or member is obsolete