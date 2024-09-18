using Android.Widget;

public static class OperationExtensions
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
            NumberPostfix.th => "th",
            _ => "th"
        };
    }
}
public enum PointType
{
    Empty,
    X,
    O,
}
public enum NumberPostfix
{
    st,
    nd,
    rd,
    th,
}