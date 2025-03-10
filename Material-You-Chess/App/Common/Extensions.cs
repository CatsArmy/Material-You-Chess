using Android.Annotation;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby.Connection;
using Android.OS;
using AndroidX.Activity;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using AndroidX.Core.Content;
using AndroidX.Credentials;
using AndroidX.Credentials.Exceptions;
using Chess.App.Common.ActivityResult;
using Java.Util;
using Microsoft.Maui.ApplicationModel;

namespace Chess.App.Common;

public static class Extensions
{
    public static ISharedPreferences? GetMaterialYouThemePreference(this Android.App.Activity app, out bool MaterialYouThemePreference)
    {
        ISharedPreferences? sharedPref = app.GetPreferences(FileCreationMode.Private);
        MaterialYouThemePreference = true;
        if (sharedPref!.Contains(nameof(MaterialYouThemePreference)))
        {
            MaterialYouThemePreference = sharedPref.GetBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference);
            return sharedPref;
        }
        var editor = sharedPref.Edit();
        editor?.PutBoolean(nameof(MaterialYouThemePreference), MaterialYouThemePreference)?.Commit();
        editor?.Apply();
        return sharedPref;
    }

    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> to, params Dictionary<TKey, TValue>[] merge) where TKey : notnull where TValue : notnull
    {
        foreach (var dictionary in merge)
            foreach (var kvp in dictionary)
            {
                if (to.ContainsKey(kvp.Key))
                    continue;

                to[kvp.Key] = kvp.Value;
            }
    }

    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where I : Java.Lang.Object
        where O : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I, O>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where I : Java.Lang.Object
        where O : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback @callback)
        where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher<I> RegisterForActivityResult<I>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback @callback)
        where I : Java.Lang.Object
        => (@base.RegisterForActivityResult(contract, callback) as ActivityResultLauncher<I>)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this ComponentActivity @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where O : Java.Lang.Object
        => @base.RegisterForActivityResult(contract, callback)!;

    public static ActivityResultLauncher RegisterForActivityResult<O>(this AndroidX.Fragment.App.Fragment @base,
        ActivityResultContract contract,
        IActivityResultCallback<O> @callback)
        where O : Java.Lang.Object
        => @base.RegisterForActivityResult(contract, callback)!;

    /// <summary> Transforms a <see cref="Statuses"/> into a English-readable message for logging. </summary>
    /// <param name="status">The current status. </param>
    /// <returns> A readable String.eg. [404] File not found. </returns>
    public static string Status(this Statuses status)
    {
        string msg = (status.StatusMessage == null) switch
        {
            true => ConnectionsStatusCodes.GetStatusCodeString(status.StatusCode),
            false => status.StatusMessage
        };
        return Java.Lang.String.Format(Locale.Us!, "[%d]%s", status.StatusCode, msg!).ToString();
    }

    public static Java.Lang.Class ToJavaClass(this Type type) => Java.Lang.Class.FromType(type);

    public static async Task<CreatePublicKeyCredentialResponse?> CreatePublicKeyCredential(this ICredentialManager credentialManager,
        CreatePublicKeyCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await credentialManager.CreateCredential(request, cancellationToken);
        return (CreatePublicKeyCredentialResponse?)response;
    }
    public static async Task<CreateCustomCredentialResponse?> CreateCustomCredential(this ICredentialManager credentialManager,
        CreateCustomCredentialRequest request, CancellationToken cancellationToken)
    {
        var response = await credentialManager.CreateCredential(request, cancellationToken);
        return (CreateCustomCredentialResponse?)response;
    }
    public static async Task<CreatePasswordResponse?> CreatePassword(this ICredentialManager credentialManager,
        CreatePasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await credentialManager.CreateCredential(request, cancellationToken);
        return (CreatePasswordResponse?)response;
    }

    [TargetApi(Value = 34)]
    public static PendingIntent CreateSettingsPendingIntent(this ICredentialManager credentialManager)
    {
        return credentialManager.CreateSettingsPendingIntent();
    }

    public static async Task<GetCredentialResponse?> GetCredentialAsync(this ICredentialManager credentialManager,
        GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>(cancellationToken);

        credentialManager.GetCredentialAsync(
            Platform.CurrentActivity!,
            request,
            null,
            ContextCompat.GetMainExecutor(Platform.CurrentActivity!),
            callback
        );

        return await callback.Task;
    }

    [TargetApi(Value = 34)]
    public static async Task<PrepareGetCredentialResponse?> PrepareGetCredential(this ICredentialManager credentialManager,
        GetCredentialRequest request, CancellationToken cancellationToken)
    {
        var callback = new CredentialManagerCallback<PrepareGetCredentialResponse, GetCredentialException>(cancellationToken);

        credentialManager.PrepareGetCredentialAsync(
            request,
            null,
            ContextCompat.GetMainExecutor(Platform.CurrentActivity!),
            callback
        );

        return await callback.Task;
    }

    public static async Task ClearCredentialState(this ICredentialManager credentialManager, CancellationToken cancellationToken)
    {
        var request = new ClearCredentialStateRequest();
        var callback = new CredentialManagerCallback<ClearCredentialException>(cancellationToken);

        var cancellationSignal = new CancellationSignal();
        cancellationToken.Register(() => cancellationSignal.Cancel());

        credentialManager.ClearCredentialStateAsync(
            request,
            cancellationSignal,
            ContextCompat.GetMainExecutor(Platform.CurrentActivity!),
            callback
        );

        await callback.Task;
    }

    private static Task<CreateCredentialResponse?> CreateCredential(this ICredentialManager credentialManager,
        CreateCredentialRequest request, CancellationToken cancellationToken, Action<GetCredentialResponse>? _callback = null)
    {
        var callback = new CredentialManagerCallback<CreateCredentialResponse, CreateCredentialException>(cancellationToken, _callback);

        credentialManager.CreateCredentialAsync(
            Platform.CurrentActivity!,
            request,
            null,
            ContextCompat.GetMainExecutor(Platform.CurrentActivity!),
            callback
        );

        return callback.Task;
    }
}
