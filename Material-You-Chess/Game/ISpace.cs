using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game;

public interface ISpace
{
    [JsonIgnore] public ImageView? SpaceView { get; }
    public (char file, int rank) Index { get; }
    public bool IsWhite { get; }
    public char File { get; }
    public int Rank { get; }
    public int Id { get; }

    //public void Select(bool inherit = false, bool hasMoves = false);
    //public void Unselect(bool inherit = false, bool hasMoves = false);


    public BoardPiece? Piece(Dictionary<(string, int), BoardPiece> boardPieces);
}
