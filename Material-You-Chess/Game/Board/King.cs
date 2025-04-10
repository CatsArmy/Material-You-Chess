using Chess.Game.Moves;

namespace Chess.Game.Board;

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
        game.Player!.Outcome = GameOutcome.Win;
        game.Enemy!.Outcome = GameOutcome.Lose;

        foreach (var view in game.AllPieces.Values) view.PieceView!.Clickable = false;
        foreach (var view in game.Board.Values) view.SpaceView!.Clickable = false;

        //display and handle the end of the game
        game.Activity.EndGame(game.Player, game.Enemy);
    }

    public List<Move> RegularMoves(ChessGame game)
    {
        var moves = (List<Move>)[];
        var up = this.Space?.Up(game.Board);
        if (up != null)
        {
            if (up.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, up));
            else if (up.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var down = this.Space?.Down(game.Board);
        if (down != null)
        {
            if (down.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, down));
            else if (down.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var left = this.Space?.Left(game.Board);
        if (left != null)
        {
            if (left.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, left));
            else if (left.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var right = this.Space?.Right(game.Board);
        if (right != null)
        {
            if (right.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, right));
            else if (right.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var topLeft = this.Space?.DiagonalUp(game.Board, false);
        if (topLeft != null)
        {
            if (topLeft.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, topLeft));
            else if (topLeft.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var topRight = this.Space?.DiagonalUp(game.Board, true);
        if (topRight != null)
        {
            if (topRight.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, topRight));
            else if (topRight.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var bottomLeft = this.Space?.DiagonalDown(game.Board, false);
        if (bottomLeft != null)
        {
            if (bottomLeft.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, bottomLeft));
            else if (bottomLeft.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        var bottomRight = this.Space?.DiagonalDown(game.Board, true);
        if (bottomRight != null)
        {
            if (bottomRight.Piece(game.AllPieces) is not BoardPiece piece)
                moves.Add(new Move(this, bottomRight));
            else if (bottomRight.Piece(game.AllPieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }

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

        //Filter by moves that can potentially threaten us
        //enemyMoves = [.. moves.Where(move => move is not MoveOnly)];

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