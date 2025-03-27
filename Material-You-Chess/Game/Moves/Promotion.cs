using System.Diagnostics.CodeAnalysis;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Promotion(Pawn origin, BoardSpace destination, SerializedType? promoteTo = null) : Move(origin, destination)
{
    public SerializedType? PromoteTo { get; set; } = promoteTo;

    public Promotion(Pawn origin, BoardSpace destination, Type typeToPromoteTo)
        : this(origin, destination, promoteTo: new(typeToPromoteTo)) { }
}

public readonly struct SerializedType
{
    public string Type { get; }

    public static bool operator ==(SerializedType serialized, Type type) => serialized.Type == $"{type}";
    public static bool operator ==(Type type, SerializedType serialized) => serialized.Type == $"{type}";

    public static bool operator !=(SerializedType serialized, Type type) => serialized.Type != $"{type}";
    public static bool operator !=(Type type, SerializedType serialized) => serialized.Type != $"{type}";

    public SerializedType(string type) => this.Type = type;

    public SerializedType(Type type) => this.Type = $"{type}";

    public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();
}
