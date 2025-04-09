using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Chess.Game.Moves;

[method: JsonConstructor]
public readonly struct SerializedType(string type)
{
    public string Type { get; } = type;

    public static bool operator ==(SerializedType serialized, Type type) => serialized.Type == $"{type}";
    public static bool operator ==(Type type, SerializedType serialized) => serialized.Type == $"{type}";

    public static bool operator !=(SerializedType serialized, Type type) => serialized.Type != $"{type}";
    public static bool operator !=(Type type, SerializedType serialized) => serialized.Type != $"{type}";

    public SerializedType(Type type) : this($"{type}") { }

    public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();
}
