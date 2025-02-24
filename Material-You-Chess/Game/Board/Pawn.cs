using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Pawn(int id, (string, int) index, bool isWhite, BoardSpace space, ConstraintLayout boardLayout)
    : SpecialPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    public bool EnPassantCapturable = false;

    private const char abbreviation = 'P';

    public override void Update(bool IsUpdatingPlayer = false)
    {
        base.Update(IsUpdatingPlayer);
        this.EnPassantCapturable = false;
    }

    public override void Move(Move move, ChessGame game)
    {
        this.Update();
        if (move is EnPassant enPassant)
        {
            game.Selected!.Capture(enPassant.Pawn, game);
            this.Move(move);
            game.NextTurn(move);
            return;
        }

        if (move is DoubleMove)
        {
            this.EnPassantCapturable = true;
        }

        if (move is Promotion promotion && promotion.PromoteTo != null)
        {
            var Piece = game.Player!.Pieces[move.OriginPiece.Index];
            if (promotion.PromoteTo?.Type == $"{typeof(Queen)}")
            {
                Piece = new Queen(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, game.BoardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.queen_white,
                    false => Resource.Drawable.queen_black
                });
            }

            else if (promotion.PromoteTo?.Type == $"{typeof(Knight)}")
            {
                Piece = new Knight(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, game.BoardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.knight_white,
                    false => Resource.Drawable.knight_black
                });
            }

            else if (promotion.PromoteTo?.Type == $"{typeof(Rook)}")
            {
                Piece = new Rook(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, game.BoardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.rook_white,
                    false => Resource.Drawable.rook_black
                });
            }

            else if (promotion.PromoteTo?.Type == $"{typeof(Bishop)}")
            {
                Piece = new Bishop(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, game.BoardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.bishop_white,
                    false => Resource.Drawable.bishop_black
                });
            }

            game.Player!.Pieces[move.OriginPiece.Index] = Piece;
            game.AllPieces![move.OriginPiece.Index] = Piece;
            if (move is PromotionCapture capture)
                Piece.Capture(capture.Piece, game);
        }

        this.Move(move);
        game.NextTurn(move);
    }

    public override List<Move> Moves(ChessGame game)
    {
        List<Move> moves = base.Moves(game);
        if (this.Space.Forward(game.Board, this.IsWhite) is not BoardSpace forward)
            return moves;
        int rank = this.IsWhite switch
        {
            true => 8,
            false => 1
        };
        var piece = forward.Piece(game.AllPieces);
        if (piece == null)
        {
            moves.Add((forward.Rank == rank) switch
            {
                true => new Promotion(this, forward),
                false => new MoveOnly(this, forward)
            });
            if (!this.HasMoved)
            {
                var forwardX2 = forward.Forward(game.Board, this.IsWhite);
                if (forwardX2?.Piece(game.AllPieces) == null)
                    moves.Add(new DoubleMove(this, forwardX2!));
            }
        }

        if (forward.Left(game.Board) is BoardSpace left)
        {
            if (left.Piece(game.AllPieces) is BoardPiece leftPiece)
            {
                if (leftPiece?.IsWhite != this.IsWhite)
                    moves.Add((left.Rank == rank) switch
                    {
                        true => new PromotionCapture(this, leftPiece!),
                        false => new Capture(this, leftPiece!)
                    });
            }
            else if (left.Backward(game.Board, isWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game.AllPieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, left, captured));
            }
        }

        if (forward.Right(game.Board) is BoardSpace right)
        {
            if (right.Piece(game.AllPieces) is BoardPiece rightPiece)
            {
                if (rightPiece?.IsWhite != this.IsWhite)
                    moves.Add((right.Rank == rank) switch
                    {
                        true => new PromotionCapture(this, rightPiece!),
                        false => new Capture(this, rightPiece!)
                    });
            }
            else if (right.Backward(game.Board, isWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game.AllPieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, right, captured));
            }
        }

        return moves;
    }
}
