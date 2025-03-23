using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

#pragma warning disable XAOBS001 // Type or member is obsolete

public class OnSuccess(Action<Java.Lang.Object?> action) : Java.Lang.Object, IOnSuccessListener
{
    void IOnSuccessListener.OnSuccess(Java.Lang.Object? result) => action(result);
}

#pragma warning restore XAOBS001 // Type or member is obsolete