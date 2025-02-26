using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteQueen(int id, int count, BoardSpace space) : Queen(id, space)
{
    public WhiteQueen(string prefix, int id, int count, BoardSpace space)
        : this(id, count, space) => this.Prefix = prefix;

    private string Prefix { get; set; } = $"w{nameof(Queen)}";

    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => true;
}

public class BlackQueen(int id, int count, BoardSpace space) : Queen(id, space)
{
    public BlackQueen(string prefix, int id, int count, BoardSpace space)
    : this(id, count, space) => this.Prefix = prefix;

    private string Prefix { get; set; } = $"b{nameof(Queen)}";

    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => false;
}

public class Queen(int id, BoardSpace space) : BoardPiece(id, space)
{
    public override char Abbreviation => 'Q';

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game.Board, game.AllPieces, ref moves);
        this.Verticals(game.Board, game.AllPieces, ref moves);
        this.Diagonals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
