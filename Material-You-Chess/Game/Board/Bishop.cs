using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteBishop(int id, int count, BoardSpace space, string prefix = $"w{nameof(Bishop)}") : Bishop(id, space)
{
    public override string Prefix => prefix;
    public override int Count => count;
    public override bool IsWhite => true;
}

public class BlackBishop(int id, int count, BoardSpace space, string prefix = $"b{nameof(Bishop)}") : Bishop(id, space)
{
    public override string Prefix => prefix;
    public override int Count => count;
    public override bool IsWhite => false;
}

public class Bishop(int id, BoardSpace space) : BoardPiece(id, space)
{
    public override char Abbreviation => 'B';

    /// <summary> Generates all available moves at this state of the game based on the rules a regular the chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        this.Diagonals(game, ref moves);
        return moves;
    }
}
