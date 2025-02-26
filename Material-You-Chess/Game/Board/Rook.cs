using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteRook(int id, int count, BoardSpace space) : Rook(id, space)
{
    public WhiteRook(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.Prefix = prefix;

    private string Prefix { get; set; } = $"w{nameof(Rook)}";
    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => true;
}

public class BlackRook(int id, int count, BoardSpace space) : Rook(id, space)
{
    public BlackRook(string prefix, int id, int count, BoardSpace space) : this(id, count, space)
        => this.Prefix = prefix;

    private string Prefix { get; set; } = $"b{nameof(Rook)}";

    public override (string prefix, int count) Index => (this.Prefix, count);
    public override bool IsWhite => false;
}

public class Rook(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override char Abbreviation => 'R';

    public override void Move(Move move, ChessGame game)
    {
        base.Move(move, game);
        this.Move(move);
        game.NextTurn(move);
    }

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game.Board, game.AllPieces, ref moves);
        this.Verticals(game.Board, game.AllPieces, ref moves);
        return moves;
    }
}
