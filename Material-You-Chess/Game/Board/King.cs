using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;
public class King(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : SpecialPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'K';

    public override void Move(Move move, ChessGame game)
    {
        base.Move(move, game);
        this.Move(move);
        game.NextTurn(move);
    }

    public override void Capture(BoardPiece destination, ChessGame game)
    {
        base.Capture(destination, game);
        game.Player!.Outcome = GameOutcome.Win;
        game.Enemy!.Outcome = GameOutcome.Lose;
        foreach (var Space in game.Board.Values)
            Space.Space!.Clickable = false;

        foreach (var piece in game.AllPieces.Values)
            piece.Space.Space!.Clickable = false;

        //display and handle the end of the game
        game.WinnerToast.Show();
    }

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);

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
}