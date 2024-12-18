using System;
using Android.Widget;

namespace Chess.Util.Logger;

public static class Log
{
    public static TextView LogView;
    private const string tag = "CatDebug";
    private static void AppendToLogs(string msg)
    {
        if (LogView == null)
            return;

        LogView.Append("\n");
        LogView.Append(DateTime.Now.ToString("hh:mm"));
        LogView.Append($" {msg}");
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

