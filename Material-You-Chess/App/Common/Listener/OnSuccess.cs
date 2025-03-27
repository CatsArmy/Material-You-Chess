using Android.Gms.Tasks;

namespace Chess.App.Common.Listener;

public class OnSuccess(Action<Java.Lang.Object?> action) : Java.Lang.Object, IOnSuccessListener
{
    void IOnSuccessListener.OnSuccess(Java.Lang.Object? result) => action(result);
}

public class OnSuccess<I>(Action<I?> action) : Java.Lang.Object, IOnSuccessListener where I : Java.Lang.Object
{

    void IOnSuccessListener.OnSuccess(Java.Lang.Object? result) => action(result as I);
}