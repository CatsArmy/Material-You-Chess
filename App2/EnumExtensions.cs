using Android.Widget;

public static class EnumExtensions
{
    public static PointType GetValue(this Button button)
    {
        const string X = "❌";
        const string O = "⭕";
        PointType type = button.Text switch
        {
            X => PointType.X,
            O => PointType.O,
            _ => PointType.Empty,
        };
        return type;
    }
    public static string GetValue(this PointType type)
    {
        const string X = "❌";
        const string O = "⭕";
        return type switch
        {
            PointType.X => X,
            PointType.O => O,
            _ => string.Empty
        };
    }
    public static string GetValue(this NumberPostfix postfix)
    {
        return postfix switch
        {
            NumberPostfix.st => "st",
            NumberPostfix.nd => "nd",
            NumberPostfix.rd => "rd",
            _ => "th"
        };
    }
    public static void SetButtonColor(this Button btn, Button background = null)
    {
        if (background is null)
        {
            btn.Background = null;
            return;
        }
        btn.Background = background.Background;
    }
}
