namespace Chess.App.Common.Extensions;

/// an extension method is a a static method that the first parameter is prefixed with the this keyword 
/// will let you call the extension method from the type of the first parameter e.g:
/// for the given method:
/// public static bool AreEven(this int value, bool isExtensionTest) => value % 2 == 0;
/// you can call it by either calling
/// int myValue1 = 45; // the name of the class containing the extension method
/// myValue1.AreEven(true); /* or */ Extensions.AreEven(myValue1, false);

/// <summary> a static class containing extension a method </summary>
public static class Extensions
{
    public static Java.Lang.Class Class(this Type type) => Java.Lang.Class.FromType(type);
}
