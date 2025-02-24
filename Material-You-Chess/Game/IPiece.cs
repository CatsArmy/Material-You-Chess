using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.Game;

public interface IPiece
{
    [JsonIgnore] public ImageView? Piece { get; set; }

    public BoardSpace Space { get; set; }

    public int Id { get; }

    public bool IsWhite { get; }

    public (string, int) Index { get; }

    public char Abbreviation { get; }

    public void Update(bool IsUpdatingPlayer = false);
    public void Move(Move destination, ChessGame game);
}
