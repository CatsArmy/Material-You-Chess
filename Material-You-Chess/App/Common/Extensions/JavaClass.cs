using System.Diagnostics.CodeAnalysis;

namespace Material.You.Chess.App.Common.Extensions;

public static class Extensions
{
    //new C# 14 .NET 10 Feature: https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14#extension-members
    extension(Type type)
    {
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "False positive")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "False positive")]
        public Java.Lang.Class Class => Java.Lang.Class.FromType(type); // Used to help register the FirebaseImageLoader
    }
}
