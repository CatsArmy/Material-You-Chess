namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    public static Java.Lang.Class Class(this Type type) => Java.Lang.Class.FromType(type);
}
