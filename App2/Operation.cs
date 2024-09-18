using Android.Widget;

public static class OperationExtensions
{
    public static PointType GetValue(this Button button)
    {
        const string X = "X";
        const string O = "O";
        PointType type;
        switch (button.Text)
        {
            case X:
                type = PointType.X;
                break;
            case O:
                type = PointType.O;
                break;
            default:
                type = PointType.Empty;
                break;
        }
        return type;
    }
    public static string GetValue(this PointType type)
    {
        string result = string.Empty;
        switch (type)
        {
            case PointType.X:
                result = "X";
                break;
            case PointType.O:
                result = "O";
                break;
            default:
                break;
        }
        return result;
    }
}
public enum PointType
{
    Empty,
    X,
    O,
}