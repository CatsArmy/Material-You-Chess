using Chess.Game.Board;

namespace Chess.Game.Moves;

//[JsonDerivedType(typeof(PromotionCapture), nameof(PromotionCapture))]
public class Promotion(Pawn origin, BoardSpace destination, SerializedType? promoteTo = null) : Move(origin, destination)
{
    public SerializedType? PromoteTo { get; set; } = promoteTo;

    public Promotion(Pawn origin, BoardSpace destination, Type promoteTo) : this(origin, destination, new SerializedType(promoteTo)) { }
}


public class SerializedType
{
    public string Type { get; }

    public SerializedType(string type)
    {
        Type = type;
    }

    public SerializedType(Type type)
    {
        Type = $"{type}";
    }
}
