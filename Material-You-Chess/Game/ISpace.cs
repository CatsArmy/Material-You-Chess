using System.Text.Json.Serialization;
using Chess.Game.Board;

namespace Chess.Game;

public interface ISpace
{
    [JsonIgnore] ImageView? SpaceView { get; }
    (char file, int rank) Index { get; }
    bool IsWhite { get; }
    char File { get; }
    int Rank { get; }
    int Id { get; }

    BoardSpace? Forward(Dictionary<(char file, int rank), BoardSpace> board, bool isWhite);
    BoardSpace? Backward(Dictionary<(char file, int rank), BoardSpace> board, bool isWhite);
    BoardSpace? DiagonalDown(Dictionary<(char file, int rank), BoardSpace> board, bool isRight);
    BoardSpace? DiagonalUp(Dictionary<(char file, int rank), BoardSpace> board, bool isRight);

    BoardSpace? Up(Dictionary<(char file, int rank), BoardSpace> board);
    BoardSpace? Down(Dictionary<(char file, int rank), BoardSpace> board);
    BoardSpace? Left(Dictionary<(char file, int rank), BoardSpace> board);
    BoardSpace? Right(Dictionary<(char file, int rank), BoardSpace> board);

    BoardPiece? Piece(Dictionary<(string Prefix, int Count), BoardPiece> boardPieces);

    void IndicateMoveable();
    void IndicateUnmovable();

    void Unselect();
    void Select();

    bool IsSelected();
    bool IsUnselected();
    bool IsSelectedMove();
    bool IsUnselectedMove();

}
