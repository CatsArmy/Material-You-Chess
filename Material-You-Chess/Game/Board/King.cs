using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Board;

public class WhiteKing(int id, int count, BoardSpace space) : King(id, space)
{
    public override int Count => count;
    public override string Prefix => $"w{nameof(King)}";
    public override bool IsWhite => true;
}

public class BlackKing(int id, int count, BoardSpace space) : King(id, space)
{
    public override int Count => count;
    public override string Prefix => $"b{nameof(King)}";
    public override bool IsWhite => false;
}

public class King(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override char Abbreviation => 'K';

    public override void Capture(ChessGame game)
    {
        base.Capture(game);
        game.EndGame(game.Player, game.Enemy);
    }

    /// <remarks> this function exists to prevent a infinite recursion loop </remarks>
    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public List<Move> RegularMoves(ChessGame game)
    {
        var moves = (List<Move>)[];
        var up = this.Space?.Up(game);
        if (up != null)
        {
            if (up.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, up));
            else if (up.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var down = this.Space?.Down(game);
        if (down != null)
        {
            if (down.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, down));
            else if (down.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var left = this.Space?.Left(game);
        if (left != null)
        {
            if (left.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, left));
            else if (left.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var right = this.Space?.Right(game);
        if (right != null)
        {
            if (right.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, right));
            else if (right.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var topLeft = this.Space?.DiagonalUpLeft(game);
        if (topLeft != null)
        {
            if (topLeft.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, topLeft));
            else if (topLeft.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var topRight = this.Space?.DiagonalUpRight(game);
        if (topRight != null)
        {
            if (topRight.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, topRight));
            else if (topRight.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var bottomLeft = this.Space?.DiagonalDownLeft(game);
        if (bottomLeft != null)
        {
            if (bottomLeft.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, bottomLeft));
            else if (bottomLeft.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var bottomRight = this.Space?.DiagonalDownRight(game);
        if (bottomRight != null)
        {
            if (bottomRight.Piece(game) is not BoardPiece piece)
                moves.Add(new Move(this, bottomRight));
            else if (bottomRight.Piece(game)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }

    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        var moves = base.Moves(game);
        moves.AddRange(this.RegularMoves(game));

        if (this.HasMoved)
            return moves;

        var enemyMoves = (List<Move>)[];
        foreach (var enemyPiece in game.Enemy!.Pieces.Values)
        {
            if (enemyPiece is King king)
            {
                enemyMoves.AddRange(king.RegularMoves(game));
                continue;
            }

            enemyMoves.AddRange(enemyPiece.Moves(game));
        }

        var rank = this.Space!.Rank;
        if (!game.Player!.Rook1!.HasMoved) //Rook1 == Queen Side
        {
            char[] files = [game.Player!.Rook1!.Space.File, 'B', 'C', 'D', this.Space.File];

            var inDanger = false;
            foreach (var file in files)
                if (game.Board[(file, rank)].IsThreateningSpace(ref enemyMoves))
                    inDanger = true;

            if (!inDanger)
                moves.Add(new QueenSideCastle(game));
        }

        if (!game.Player!.Rook2!.HasMoved) //Rook2 == King Side
        {
            char[] files = [this.Space.File, 'F', 'G', game.Player!.Rook2!.Space.File];

            var inDanger = false;
            foreach (var file in files)
                if (game.Board[(file, rank)].IsThreateningSpace(ref enemyMoves))
                    inDanger = true;

            if (!inDanger)
                moves.Add(new KingSideCastle(game));
        }

        return moves;
    }
}