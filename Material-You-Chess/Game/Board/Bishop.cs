using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteBishop(int id, int count, BoardSpace space) : Bishop(id, space)
{
    public WhiteBishop(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override string Prefix => this.prefix;
    public override int Count => count;
    public override bool IsWhite => true;
    private string prefix { get; set; } = $"w{nameof(Bishop)}";
}

public class BlackBishop(int id, int count, BoardSpace space) : Bishop(id, space)
{
    public BlackBishop(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override string Prefix => this.prefix;
    public override int Count => count;
    public override bool IsWhite => false;
    private string prefix { get; set; } = $"b{nameof(Bishop)}";
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
