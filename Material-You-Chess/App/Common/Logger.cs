using Log = Android.Util.Log;

namespace Chess.App.Common;

#pragma warning disable CS0162 // Unreachable code detected
public static class Logger
{
    private enum Level
    {
        Verbose,
        Debug,
        Warn,
        Error,
        Fatal
    }
    private const Level Filter = Level.Debug;
    private const string Tag = "CatDebug";

    public static void Verbose(string message)
    {
        if (Filter > Level.Verbose)
            return;
        Log.Verbose(Tag, $"{message}{Environment.NewLine}");
    }

    public static void Debug(string message)
    {
        if (Filter > Level.Debug)
            return;
        Log.Debug(Tag, $"{message}{Environment.NewLine}");
    }

    public static void Warn(string message)
    {
        if (Filter > Level.Warn)
            return;
        Log.Warn(Tag, $"{message}{Environment.NewLine}");
    }

    public static void Error(string message)
    {
        if (Filter > Level.Error)
            return;
        Log.Error(Tag, $"{message}{Environment.NewLine}");
    }
}
#pragma warning restore CS0162 // Unreachable code detected