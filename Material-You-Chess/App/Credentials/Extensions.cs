using Android.Content;

namespace Chess.App.Credentials;

public static class Extensions
{
    public static string readFromAsset(this Context context, string asset)
    {
        return string.Empty;
    }
    public static void showErrorAlert(this Context context, string msg)
    {
        new AlertDialog.Builder(context)
            !.SetTitle("An error occurred")
            !.SetMessage(msg)
            !.SetNegativeButton("Ok", handler: null)
            !.Show();
    }
}
