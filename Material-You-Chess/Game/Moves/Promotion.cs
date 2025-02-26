using Chess.Game.Board;

namespace Chess.Game.Moves;

public class Promotion(Pawn origin, BoardSpace destination, SerializedType? promoteTo = null) : Move(origin, destination)
{
    public SerializedType? PromoteTo { get; set; } = promoteTo;

    public Promotion(Pawn origin, BoardSpace destination, Type typeToPromoteTo)
        : this(origin, destination, promoteTo: new(typeToPromoteTo)) { }
}


/// Type defines operator == or operator != but does not override Object.GetHashCode()
/// Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0660, CS0661 
public readonly struct SerializedType
#pragma warning restore CS0660, CS0661
{
    public string Type { get; }

    public static bool operator ==(SerializedType serialized, Type type) => serialized.Type == $"{type}";
    public static bool operator ==(Type type, SerializedType serialized) => serialized.Type == $"{type}";

    public static bool operator !=(SerializedType serialized, Type type) => serialized.Type != $"{type}";
    public static bool operator !=(Type type, SerializedType serialized) => serialized.Type != $"{type}";

    public SerializedType(string type)
    {
        Type = type;
    }

    public SerializedType(Type type)
    {
        Type = $"{type}";
    }
}
