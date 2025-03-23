using Android.Gms.Common.Apis;
using Android.Gms.Nearby.Connection;
using Java.Util;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
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
}

