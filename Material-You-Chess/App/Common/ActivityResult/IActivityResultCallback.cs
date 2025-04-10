using Android.Runtime;
using AndroidX.Activity.Result;

namespace Chess.App.Common.ActivityResult;

public interface IActivityResultCallback<O> : IActivityResultCallback where O : class, IJavaObject
{
    public void OnActivityResult(O? result);
}
