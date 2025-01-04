using Android.Content;
using Android.Gms.Extensions;
using Android.Util;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Data;
using Bumptech.Glide.Load.Model;
using Bumptech.Glide.Module;
using Firebase.Storage;
using Java.IO;
using Java.Lang;
using Java.Nio.Charset;
using Java.Security;

namespace Chess;

public class MyAppGlideModule : AppGlideModule//, IGlideModule
{
    private InputStream type { get; }
    public override void RegisterComponents(Context context, Glide glide, Registry registry)
    {
        // Register FirebaseImageLoader to handle StorageReference
        registry.Append(FirebaseStorage.Instance.Reference.Class, type.Class,
                new FirebaseImageLoader.Factory());
    }
}

public class FirebaseImageLoader/*<Model, Data>*/ : Java.Lang.Object, IModelLoader /*where Model : StorageReference where Data : InputStream*/
{
    private const string Tag = "FirebaseImageLoader";

    public class Factory : Java.Lang.Object, IModelLoaderFactory
    {
        public IModelLoader Build(MultiModelLoaderFactory factory)
        {
            return new FirebaseImageLoader();
        }

        public void Teardown() { }
    }

    public ModelLoaderLoadData BuildLoadData(Java.Lang.Object p0, int height, int width, Options options)
    {
        var reference = p0 as StorageReference;
        return new ModelLoaderLoadData(new FirebaseStorageKey(reference!), new FirebaseStorageFetcher(reference!));
    }

    public bool Handles(Java.Lang.Object reference) => true;

    private class FirebaseStorageKey : Java.Lang.Object, Bumptech.Glide.Load.IKey
    {

        private StorageReference mRef;

        public FirebaseStorageKey(StorageReference _ref)
        {
            mRef = _ref;
        }

        public void UpdateDiskCacheKey(MessageDigest digest)
        {
            digest.Update(new Java.Lang.String(mRef.Path).GetBytes(Charset.DefaultCharset()!)!);
        }

        public override bool Equals(Java.Lang.Object? o)
        {
            if (this == o)
                return true;

            if (o == null || base.Class != o.Class)
                return false;

            FirebaseStorageKey key = (FirebaseStorageKey)o;

            return mRef.Equals(key.mRef);
        }

        public override int GetHashCode()
        {
            return mRef.GetHashCode();
        }
    }

    private class FirebaseStorageFetcher(StorageReference _ref) : Java.Lang.Object, IDataFetcher
    {
        public Class DataClass => ((Android.Runtime.InputStreamInvoker)mInputStream).BaseInputStream.Class;
        public DataSource DataSource => DataSource.Remote!;

        private StorageReference mRef = _ref;
        private StreamDownloadTask mStreamTask;
        private Stream mInputStream;

        public async void LoadData(Priority priority, IDataFetcherDataCallback callback)
        {
            mStreamTask = mRef.Stream;
            var task = mStreamTask.AsAsync<StreamDownloadTask.TaskSnapshot>();
            var snapshot = await task;
            if (task.IsCompletedSuccessfully) //OnSuccess
            {
                mInputStream = snapshot.Stream;
                callback.OnDataReady(((Android.Runtime.InputStreamInvoker)mInputStream).BaseInputStream);
            }
            if (task.IsFaulted) //OnFailure
            {
                callback.OnLoadFailed(new Java.Lang.Exception(task.Exception!.ToString()));
            }
        }

        public void Cancel()
        {
            // Close stream if possible
            if (mInputStream != null)
            {
                try
                {
                    mInputStream.Close();
                    mInputStream = null;
                }
                catch (System.IO.IOException e)
                {
                    Log.Warn(Tag, "Could not close stream", e);
                }
            }
        }

        public void Cleanup()
        {

        }
    }
}
