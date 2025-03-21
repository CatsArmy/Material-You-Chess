using Chess.App.Common.ActivityResult;

namespace Chess.App.Common;

public class RequestPermissionCallback(Action<bool> OnRequestCallback) : Java.Lang.Object, IActivityResultCallback<IMap<string, bool>>
{
    //<I>: string[], <O>: Dictionary<string, bool>>
    public void OnActivityResult(Java.Lang.Object? result) => this.OnActivityResult((result as IMap<string, bool>));
    public void OnActivityResult(IMap<string, bool>? permissions)
    {
        foreach (var isGranted in permissions!.Values().Cast<bool>().ToArray())
        {
            if (!isGranted)
            {
                // Explain to the user that the feature is unavailable because the
                // feature requires a permission that the user has denied. At the
                // same time, respect the user's decision. Don't link to system
                // settings in an effort to convince the user to change their decision.
                OnRequestCallback(false);
                return;
            }

            // Permission is granted. Continue the action or workflow in your app.
            OnRequestCallback(true);
        }
    }
}
