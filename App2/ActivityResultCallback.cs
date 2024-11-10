using System.Threading.Tasks;
using AndroidX.Activity.Result;
using Java.Lang;

namespace Chess
{
    public class ActivityResultCallback<O> : Object, IActivityResultCallback where O : Object
    {
        private readonly System.Action<O> _callback;
        public ActivityResultCallback(System.Action<O> callback) => _callback = callback;
        public ActivityResultCallback(TaskCompletionSource<O> tcs) => _callback = tcs.SetResult;
        public void OnActivityResult(Object p0) => _callback((O)p0);
        //[ActivityResultCallback<O>] O is of type: [Android.Net.Uri]
        //[PhotoPicker] content://media/picker/0/com.android.providers.media.photopicker/media/1000027822
    }
}