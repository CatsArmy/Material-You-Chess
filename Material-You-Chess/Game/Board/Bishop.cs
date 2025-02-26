using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteBishop(int id, int count, BoardSpace space) : Bishop(id, space)
{
    public WhiteBishop(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.Prefix = prefix;

    private string Prefix { get; set; } = $"w{nameof(Bishop)}";
    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => true;
}

public class BlackBishop(int id, int count, BoardSpace space) : Bishop(id, space)
{
    public BlackBishop(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.Prefix = prefix;

    private string Prefix { get; set; } = $"b{nameof(Bishop)}";
    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => false;
}

public class Bishop(int id, BoardSpace space) : BoardPiece(id, space)
{
    public override char Abbreviation => 'B';

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Diagonals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
