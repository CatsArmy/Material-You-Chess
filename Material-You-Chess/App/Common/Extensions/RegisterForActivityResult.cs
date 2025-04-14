using Android.Runtime;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using Chess.App.Common.ActivityResult;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Chess.App.Common.Extensions;

/// <summary> a static class containing extension methods </summary>
public static class RegisterForActivityResultOfT
{
    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this ComponentActivity @base, ActivityResultContract contract,
        IActivityResultCallback<O> @callback) where I : Java.Lang.Object where O : class, IJavaObject
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this Fragment @base, ActivityResultContract contract,
        IActivityResultCallback<O> @callback) where I : Java.Lang.Object where O : class, IJavaObject
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this ComponentActivity @base, ActivityResultContract contract,
        IActivityResultCallback @callback) where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this Fragment @base, ActivityResultContract contract,
        IActivityResultCallback @callback) where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this ComponentActivity @base, ActivityResultContract contract,
        IActivityResultCallback<O> @callback) where O : class, IJavaObject
        => @base.RegisterForActivityResult(contract, callback)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this Fragment @base, ActivityResultContract contract,
        IActivityResultCallback<O> @callback) where O : class, IJavaObject
        => @base.RegisterForActivityResult(contract, callback)!;
}
