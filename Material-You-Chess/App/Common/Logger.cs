using Log = Android.Util.Log;

namespace Chess.App.Common;

public static class Logger
{
    private const string Tag = "CatDebug";
    public static void Verbose(string message) => Log.Verbose(Logger.Tag, message);
    public static void Debug(string message) => Log.Debug(Logger.Tag, message);
    public static void Error(string message) => Log.Error(Logger.Tag, message);
    public static void Warn(string message) => Log.Warn(Logger.Tag, message);
}