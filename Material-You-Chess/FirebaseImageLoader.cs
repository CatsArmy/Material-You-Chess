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
using JavaString = Java.Lang.String;

namespace Chess;


internal class RuntimeClass : InputStream
{
    public RuntimeClass() : base() { }

    public override int Read() => throw new NotImplementedException();
}

public class MyAppGlideModule : AppGlideModule
{
    public override void RegisterComponents(Context context, Glide glide, Registry registry)
    {
        // Register FirebaseImageLoader to handle StorageReference
        registry.Append(FirebaseStorage.Instance.Reference.Class, new RuntimeClass().Class,
                new FirebaseImageLoader.Factory());
    }
}

public class FirebaseImageLoader : Java.Lang.Object, IModelLoader
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

    public ModelLoaderLoadData BuildLoadData(Java.Lang.Object _reference, int height, int width, Options options)
    {
        var reference = _reference as StorageReference;
        return new ModelLoaderLoadData(new FirebaseStorageKey(reference!), new FirebaseStorageFetcher(reference!));
    }

    public bool Handles(Java.Lang.Object reference) => true;

    private class FirebaseStorageKey(StorageReference _ref) : Java.Lang.Object, Bumptech.Glide.Load.IKey
    {
        private readonly StorageReference storageReference = _ref;

        public void UpdateDiskCacheKey(MessageDigest digest) => digest.Update(new JavaString(this.storageReference.Path).GetBytes(Charset.DefaultCharset()!)!);

        public override bool Equals(Java.Lang.Object? o)
        {
            if (this == o)
                return true;

            if (o == null || base.Class != o.Class)
                return false;

            FirebaseStorageKey key = (FirebaseStorageKey)o;

            return this.storageReference.Equals(key.storageReference);
        }

        public override int GetHashCode()
        {
            return this.storageReference.GetHashCode();
        }
    }

    private class FirebaseStorageFetcher(StorageReference _ref) : Java.Lang.Object, IDataFetcher
    {
        public Class DataClass => new RuntimeClass().Class;
        public DataSource DataSource => DataSource.Remote!;

        private readonly StorageReference storageReference = _ref;
        private StreamDownloadTask? streamTask;
        private Stream? inputStream;

        public async void LoadData(Priority priority, IDataFetcherDataCallback callback)
        {
            this.streamTask = this.storageReference.Stream;
            var task = this.streamTask.AsAsync<StreamDownloadTask.TaskSnapshot>();
            var snapshot = await task;
            if (task.IsCompletedSuccessfully) //OnSuccess
            {
                this.inputStream = snapshot.Stream;
                callback.OnDataReady(((Android.Runtime.InputStreamInvoker)this.inputStream).BaseInputStream);
            }

            if (task.IsFaulted) //OnFailure
            {
                callback.OnLoadFailed(new Java.Lang.Exception(task.Exception!.ToString()));
            }
        }

        public void Cancel()
        {
            if (this.inputStream == null)
            {
                return;
            }

            // Close stream if possible     
            try
            {
                this.inputStream.Close();
                this.inputStream = null;
            }
            catch (System.IO.IOException e)
            {
                Log.Warn(Tag, "Could not close stream", e);
            }
        }

        public void Cleanup() { }
    }
}
