using Android.Gms.Extensions;
using Android.Util;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Data;
using Bumptech.Glide.Load.Model;
using Firebase.Storage;
using Java.IO;
using Java.Lang;
using Java.Nio.Charset;
using Java.Security;
using JavaString = Java.Lang.String;

namespace Chess.App.Common;

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

    public ModelLoaderLoadData? BuildLoadData(Java.Lang.Object _reference, int height, int width, Options options)
    {
        var reference = _reference as StorageReference;
        return new ModelLoaderLoadData(new FirebaseStorageKey(reference!), new FirebaseStorageFetcher(reference!));
    }

    public bool Handles(Java.Lang.Object reference) => true;

    private class FirebaseStorageKey(StorageReference _ref) : Java.Lang.Object, Bumptech.Glide.Load.IKey
    {
        private readonly StorageReference storageReference = _ref;

        public void UpdateDiskCacheKey(MessageDigest digest) => digest.Update(new JavaString(storageReference.Path).GetBytes(Charset.DefaultCharset()!)!);

        public override bool Equals(Java.Lang.Object? o)
        {
            if (this == o)
                return true;

            if (o == null || Class != o.Class)
                return false;

            FirebaseStorageKey key = (FirebaseStorageKey)o;

            return storageReference.Equals(key.storageReference);
        }

        public override int GetHashCode()
        {
            return storageReference.GetHashCode();
        }
    }

    private class FirebaseStorageFetcher(StorageReference _ref) : Java.Lang.Object, IDataFetcher
    {
        public Class DataClass => Class.FromType(typeof(InputStream))!;
        public DataSource DataSource => DataSource.Remote!;

        private readonly StorageReference storageReference = _ref;
        private StreamDownloadTask? streamTask;
        private Stream? inputStream;


        public async void LoadData(Priority priority, IDataFetcherDataCallback callback)
        {
            try
            {
                streamTask = storageReference.Stream;
                var task = streamTask.AsAsync<StreamDownloadTask.TaskSnapshot>();
                var snapshot = await task;
                if (task.IsCompletedSuccessfully) //OnSuccess
                {
                    inputStream = snapshot.Stream;
                    callback.OnDataReady((inputStream as Android.Runtime.InputStreamInvoker)?.BaseInputStream);
                }

                if (task.IsFaulted) //OnFailure
                {
                    callback.OnLoadFailed(new Java.Lang.Exception(task.Exception!.ToString()));
                }
            }

            catch (System.Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        public void Cancel()
        {
            if (streamTask == null)
                return;

            if (!streamTask.IsInProgress)
                return;

            streamTask.Cancel();
        }

        public void Cleanup()
        {
            if (inputStream == null)
                return;

            try
            {
                inputStream.Close();
                inputStream = null;
            }
            catch (Java.IO.IOException e)
            {
                Log.Warn(Tag, "Could not close stream", e);
            }
        }
    }
}
