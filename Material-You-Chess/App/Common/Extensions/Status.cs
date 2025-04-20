using Android.Gms.Nearby.Connection;
using Java.Util;
using Api = Android.Gms.Common.Apis;

namespace Material.You.Chess.App.Common.Extensions;

public static class Statuses
{
    extension(Api.Statuses status)
    {
        /// <summary> Transforms a Statuses into a English-readable message for logging. </summary>
        /// <returns> A readable String.eg. [404] File not found. </returns>
        public string ForrmatedStatus
        {
            get
            {
                return Java.Lang.String.Format(Locale.Us!, "[%d]%s", status.StatusCode, ((status.StatusMessage == null) switch
                {
                    true => ConnectionsStatusCodes.GetStatusCodeString(status.StatusCode),
                    false => status.StatusMessage
                })!).ToString();
            }
        }
    }
}
