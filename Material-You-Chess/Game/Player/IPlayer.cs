using Chess.Dialogs;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.Game.Player;

public interface IPlayer
{
    public string Name { get; }
    public GameOutcome? Outcome { get; set; }
    public IPromotionDialog PromotionDialog { get; }

    public Dictionary<(string Prefix, int Count), BoardPiece> Pieces { get; }

    public List<Pawn> Pawns { get; }

    public Rook? Rook1 { get; set; }

    public Knight? Knight1 { get; set; }

    public Bishop? Bishop1 { get; set; }

    public King? King { get; set; }

    public Queen? Queen { get; set; }

    public Bishop? Bishop2 { get; set; }

    public Knight? Knight2 { get; set; }

    public Rook? Rook2 { get; set; }

    /// <summary>
    /// when setting the <seealso cref="Selected"/> <see cref="BoardPiece"/>
    /// it will also set the <seealso cref="Moves"/> to
    /// </summary>
    public BoardPiece? Selected { get; set; }

    /// <summary>
    /// when setting the <seealso cref="Moves" /> <see cref="BoardSpace"/> 
    /// it may call any of the following methods accordingly
    /// <br /> <see cref="Move.Select"/>
    /// <br /> <see cref="Move.Unselect"/>
    /// <br /> <see cref="Move.IndicateMoveable"/>
    /// <br /> <see cref="Move.UnindicateMoveable"/>
    /// </summary>
    public List<Move>? Moves { get; set; }

    /// <summary> When trying to set the <see langword="value" /> to <see langword="null"/>
    /// it will only clear the selected spaces and keep the last move for later use. </summary> <remarks> 
    ///     <seealso cref="LastMove"/> will be null only at the start of the game when the player has not played any moves yet.
    /// </remarks>
    public Move? LastMove { get; set; }
}
