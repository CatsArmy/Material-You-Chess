using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Board;

public class WhiteRook(int id, int count, BoardSpace space, string prefix = $"w{nameof(Rook)}") : Rook(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
    public override bool IsWhite => true;
}

public class BlackRook(int id, int count, BoardSpace space, string prefix = $"b{nameof(Rook)}") : Rook(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
    public override bool IsWhite => false;
}

public class Rook(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override char Abbreviation => 'R';

    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Horizontals(game, ref moves);
        this.Verticals(game, ref moves);
        return moves;
    }
}
