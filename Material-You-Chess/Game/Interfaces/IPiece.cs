using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.Game.Interfaces;

public interface IPiece
{
    [JsonIgnore] public ImageView? PieceView { get; set; }

    public BoardSpace Space { get; set; }

    public int Id { get; }

    public bool IsWhite { get; }

    public (string prefix, int count) Index { get; }

    public char Abbreviation { get; }

    public void Move(Move destination, ChessGame game);
    public void Update();
}
