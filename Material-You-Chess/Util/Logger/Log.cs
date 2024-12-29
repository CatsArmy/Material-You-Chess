namespace Chess.Util.Logger;

public static class Log
{
    private static TextView? logView;
    private const string tag = "CatDebug";
    public static void SetLogView(TextView? _logView)
    {
        logView = _logView;
    }

    private static void AppendToLogs(string msg)
    {
        if (logView == null)
            return;

        logView.Append("\n");
        logView.Append(DateTime.Now.ToString("hh:mm"));
        logView.Append($" {msg}");
    }

    //public static void Verbose(string message) { }
    public static void Verbose(string message)
    {
        Android.Util.Log.Verbose(tag, message);
        AppendToLogs(message);
    }
    public static void Debug(string message)
    {
        Android.Util.Log.Debug(tag, message);
        AppendToLogs(message);
    }
    public static void Error(string message)
    {
        Android.Util.Log.Error(tag, message);
        AppendToLogs(message);
    }
    public static void Warn(string message)
    {
        Android.Util.Log.Warn(tag, message);
        AppendToLogs(message);
    }
}

