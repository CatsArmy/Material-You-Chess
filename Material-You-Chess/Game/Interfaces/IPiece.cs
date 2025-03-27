using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.Game.Interfaces;

public interface IPiece
{
    [JsonIgnore] ImageView? PieceView { get; set; }
    BoardSpace Space { get; set; }
    int Id { get; }
    bool IsWhite { get; }
    (string prefix, int count) Index { get; }
    char Abbreviation { get; }
    string Prefix { get; }
    int Count { get; }
    BoardSpace? LastSpace { get; set; }

    /// <summary> <see langword="this"/> <see cref="BoardPiece"/> is the <see cref="Move.Origin"/> </summary>
    /// <remarks> <see langword="this"/> is not the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </remarks>
    /// <param name="destination"> is the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </param>
    void Capture(BoardPiece destination, ChessGame game);

    /// <summary> <see langword="this"/> <see cref="BoardPiece"/> is the <see cref="Move.Destination"/> </summary>
    /// <remarks> <see langword="this"/> is the <see cref="BoardPiece"/> that will be <see cref="Capture(ChessGame)"/>'d </remarks>
    void Capture(ChessGame game);

    void Move(Move destination, ChessGame game);

    /// <remarks> Visually moves the piece </remarks>
    /// <summary> <see langword="this"/> is the <see cref="BoardPiece"/> that will be moved to the <paramref name="move"/> </summary>
    void Move(Move move);
    List<Move> Moves(ChessGame game);
    void Update();
}
