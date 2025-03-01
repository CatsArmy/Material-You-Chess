using Chess.Game.Moves;

namespace Chess.Game.Board;

public class WhiteQueen(int id, int count, BoardSpace space, string prefix = $"w{nameof(Queen)}") : Queen(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
    public override bool IsWhite => true;
}

public class BlackQueen(int id, int count, BoardSpace space, string prefix = $"b{nameof(Queen)}") : Queen(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
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
