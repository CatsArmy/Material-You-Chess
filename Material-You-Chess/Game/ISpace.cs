using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game;

public interface ISpace
{
    [JsonIgnore] public ImageView? Space { get; }
    public (char, int) Index { get; }
    public char File { get; }
    public int Rank { get; }
    public bool IsWhite { get; }
    public int Id { get; }

    public void Select();
    public void Unselect();

    public BoardPiece? Piece(Dictionary<(string, int), BoardPiece> boardPieces);
}
