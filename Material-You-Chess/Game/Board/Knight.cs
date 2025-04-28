using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Board;

public class WhiteKnight(int id, int count, BoardSpace space, string prefix = $"w{nameof(Knight)}") : Knight(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
    public override bool IsWhite => true;
}

public class BlackKnight(int id, int count, BoardSpace space, string prefix = $"b{nameof(Knight)}") : Knight(id, space)
{
    public override int Count => count;
    public override string Prefix => prefix;
    public override bool IsWhite => false;
}

public class Knight(int id, BoardSpace space) : BoardPiece(id, space)
{
    public override char Abbreviation => 'N';

    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        var moves = base.Moves(game);
        if (Space.Up(game)?.Up(game)?.Right(game) is BoardSpace upRight)
        {
            if (upRight.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, upRight));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }
        if (Space.Up(game)?.Up(game)?.Left(game) is BoardSpace upLeft)
        {
            if (upLeft.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, upLeft));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Down(game)?.Down(game)?.Right(game) is BoardSpace downRight)
        {
            if (downRight.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, downRight));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Down(game)?.Down(game)?.Left(game) is BoardSpace downLeft)
        {
            if (downLeft.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, downLeft));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Right(game)?.Right(game)?.Up(game) is BoardSpace rightUp)
        {
            if (rightUp.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, rightUp));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Right(game)?.Right(game)?.Down(game) is BoardSpace rightDown)
        {
            if (rightDown.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, rightDown));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Left(game)?.Left(game)?.Up(game) is BoardSpace leftUp)
        {
            if (leftUp.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, leftUp));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        if (Space.Left(game)?.Left(game)?.Down(game) is BoardSpace leftDown)
        {
            if (leftDown.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, leftDown));
            else if (piece.IsWhite != IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }
}
