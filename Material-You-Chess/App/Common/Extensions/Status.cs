using Android.Gms.Nearby.Connection;
using Java.Util;
using Api = Android.Gms.Common.Apis;

namespace Material.You.Chess.App.Common.Extensions;

public static class Statuses
{
    //new C# 14 .NET 10 Feature: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14#extension-members
    extension(Api.Statuses status)
    {
        /// <summary> Transforms a Statuses into a English-readable message for logging. </summary>
        /// <returns> A readable String.eg. [404] File not found. </returns>
        public string FormattedStatus
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
