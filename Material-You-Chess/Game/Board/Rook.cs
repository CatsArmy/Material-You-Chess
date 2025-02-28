using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteRook(int id, int count, BoardSpace space) : Rook(id, space)
{
    public WhiteRook(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override int Count => count;
    public override string Prefix => this.prefix;
    public override bool IsWhite => true;
    private string prefix { get; set; } = $"w{nameof(Rook)}";
}

public class BlackRook(int id, int count, BoardSpace space) : Rook(id, space)
{
    public BlackRook(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override int Count => count;
    public override string Prefix => this.prefix;
    public override bool IsWhite => false;
    private string prefix { get; set; } = $"b{nameof(Rook)}";
}

public class Rook(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override char Abbreviation => 'R';

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game.Board, game.AllPieces, ref moves);
        this.Verticals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
