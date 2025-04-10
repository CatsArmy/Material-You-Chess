using Log = Android.Util.Log;

namespace Chess.App.Common;

public static class Logger
{
    private const string Tag = "CatDebug";
    public static void Verbose(string message) => Log.Verbose(Tag, $"{message}{Environment.NewLine}");
    public static void Debug(string message) => Log.Debug(Tag, $"{message}{Environment.NewLine}");
    public static void Error(string message) => Log.Error(Tag, $"{message}{Environment.NewLine}");
    public static void Warn(string message) => Log.Warn(Tag, $"{message}{Environment.NewLine}");
}