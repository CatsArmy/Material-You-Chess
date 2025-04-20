using Android.Content;
using Bumptech.Glide;
using Firebase.Storage;
using Java.IO;

namespace Material.You.Chess.App.Common.Extensions;

/// <summary> a static class containing extension a method </summary>
public static class FirebaseImageLoader
{
    public static void RegisterComponents(this ContextWrapper context) => Glide.Get(context).RegisterComponents();

    public static void RegisterComponents(this Glide glide) => glide.Registry.Append(typeof(StorageReference).Class, typeof(InputStream).Class, new FirebaseUI.Storage.Images.FirebaseImageLoader.Factory());
}
