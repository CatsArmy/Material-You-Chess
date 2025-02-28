using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteQueen(int id, int count, BoardSpace space) : Queen(id, space)
{
    public WhiteQueen(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override int Count => count;
    public override string Prefix => this.prefix;
    public override bool IsWhite => true;
    private string prefix { get; set; } = $"w{nameof(Queen)}";
}

public class BlackQueen(int id, int count, BoardSpace space) : Queen(id, space)
{
    public BlackQueen(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.prefix = prefix;

    public override int Count => count;
    //[JsonPropertyOrder(-1)]
    public override string Prefix => this.prefix;
    public override bool IsWhite => false;

    private string prefix { get; set; } = $"b{nameof(Queen)}";
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
