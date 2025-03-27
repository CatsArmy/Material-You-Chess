using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.App.Common.Extensions;

public static partial class Extensions
{
    /// <returns> whether or not an enemy piece can capture a (theoretical) piece 
    /// that would be placed on the given space </returns>
    public static bool IsThreateningSpace(this BoardSpace space, ref List<Move> enemy)
    {
        return enemy.FirstOrDefault(move => move.Destination == space) != null;
    }
}
