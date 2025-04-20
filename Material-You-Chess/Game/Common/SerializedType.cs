using System.Text.Json.Serialization;

namespace Material.You.Chess.Game.Common;

/// <summary>
/// Class diagram doesn't let me add this im sorry,
/// this class is just a way to pass the type of a object with json/strings
/// </summary>
/// <param name="Type">the string form of the type</param>
[method: JsonConstructor]
public readonly record struct SerializedType([property: JsonInclude] string Type)
{
    public SerializedType(Type type) : this($"{type}") { }

    public static bool operator ==(SerializedType serialized, Type type) => serialized.Type == $"{type}";
    public static bool operator ==(Type type, SerializedType serialized) => serialized.Type == $"{type}";

    public static bool operator !=(SerializedType serialized, Type type) => serialized.Type != $"{type}";
    public static bool operator !=(Type type, SerializedType serialized) => serialized.Type != $"{type}";
}
