using Android.Content;
using Bumptech.Glide;
using Firebase.Storage;
using FirebaseUI.Storage.Images;
using Java.IO;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    public static void RegisterComponents(this ContextWrapper context) => RegisterComponents(Glide.Get(context));

    public static void RegisterComponents(this Glide glide) => glide.Registry.Append(typeof(StorageReference).Class(), typeof(InputStream).Class(), new FirebaseImageLoader.Factory());
}
