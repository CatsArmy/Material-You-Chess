using Chess.Game.Common;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Pawn(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override char Abbreviation => 'P';
    public bool EnPassantCapturable = false;

    public override void Move(Move move, ChessGame game)
    {
        if (move is DoubleMove)
        {
            this.EnPassantCapturable = true;
        }

        else if (move is Promotion promotion)
        {
            if (promotion.PromoteTo is null)
            {
                game.Player!.PromotionDialog.Show(game, this, promotion);
                return;
            }

            this.Promote(game, promotion);
        }

        base.Move(move, game);
    }

    /// <summary>
    /// When the pawn is captured remove it from the list of pawns
    /// </summary>
    /// <param name="game"></param>
    public override void Capture(ChessGame game)
    {
        base.Capture(game);
        game.Enemy!.Pawns.Remove(this);
    }

    /// <summary>
    /// when this function is called it is either called from a WhitePawn class or a BlackPawn class which overrides the this function 
    /// and promotes the pawn to a queen/knight/rook/bishop with the same color as the class of the pawn that overwrote this function
    /// </summary>
    public virtual void Promote(ChessGame game, Promotion move) => game.Player!.Pawns.Remove(this);

    public override void Update()
    {
        if (this.HasMoved)
            this.EnPassantCapturable = false;

        base.Update();
    }
}

public class WhitePawn(int id, int count, BoardSpace space) : Pawn(id, space)
{
    public override string Prefix => $"w{nameof(Pawn)}";
    public override int Count => count;
    public override bool IsWhite => true;

    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        var moves = base.Moves(game);
        if (this.Space.Forward(game, this.IsWhite) is not BoardSpace forward)
            return moves;

        const int maxRank = 8;
        var piece = forward.Piece(game);
        if (piece == null)
        {
            moves.Add((forward.Rank == maxRank) switch
            {
                true => new Promotion(this, forward),
                false => new MoveOnly(this, forward)
            });

            if (!this.HasMoved)
            {
                var doubleMove = forward.Forward(game, this.IsWhite);
                if (doubleMove?.Piece(game) == null)
                    moves.Add(new DoubleMove(this, doubleMove!));
            }
        }

        if (forward.Left(game) is BoardSpace left)
        {
            if (left.Piece(game) is BoardPiece leftPiece)
            {
                if (!leftPiece.IsWhite)
                    moves.Add((left.Rank == maxRank) switch
                    {
                        true => new PromotionCapture(this, leftPiece),
                        false => new Capture(this, leftPiece)
                    });
            }
            else if (left.Backward(game, this.IsWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, left, captured));
            }
        }

        if (forward.Right(game) is BoardSpace right)
        {
            if (right.Piece(game) is BoardPiece rightPiece)
            {
                if (!rightPiece.IsWhite)
                    moves.Add((right.Rank == maxRank) switch
                    {
                        true => new PromotionCapture(this, rightPiece),
                        false => new Capture(this, rightPiece)
                    });
            }
            else if (right.Backward(game, this.IsWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, right, captured));
            }
        }

        return moves;
    }

    /// <summary> handles promoting a white pawn to a white queen/knight/rook/bishop </summary>
    public override void Promote(ChessGame game, Promotion move)
    {
        if (move.PromoteTo is not SerializedType promoteTo) throw new Exception("Failed to promote to type");

        BoardPiece Piece;
        if (promoteTo == typeof(WhiteQueen))
        {
            Piece = new WhiteQueen(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.white_queen);
        }

        else if (promoteTo == typeof(WhiteKnight))
        {
            Piece = new WhiteKnight(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.white_knight);
        }

        else if (promoteTo == typeof(WhiteBishop))
        {
            Piece = new WhiteBishop(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.white_bishop);
        }

        else if (promoteTo == typeof(WhiteRook))
        {
            Piece = new WhiteRook(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.white_rook);
        }
        else throw new Exception("Failed to promote to type");
        game.Player!.Pieces.Remove(this.Index);
        game.AllPieces.Remove(this.Index);
        game.Player!.Pieces[this.Index] = Piece;
        game.AllPieces[this.Index] = Piece;
        base.Promote(game, move);
        if (move is PromotionCapture capture)
            Piece.Capture(capture.Piece, game);
    }
}

public class BlackPawn(int id, int count, BoardSpace space) : Pawn(id, space)
{
    public override string Prefix => $"b{nameof(Pawn)}";
    public override int Count => count;
    public override bool IsWhite => false;

    /// <summary> Generates all available moves at this state of the game based on the rules of a regular chess game </summary>
    public override List<Move> Moves(ChessGame game)
    {
        var moves = base.Moves(game);
        if (this.Space.Forward(game, this.IsWhite) is not BoardSpace forward)
            return moves;

        const int maxRank = 1;
        var piece = forward.Piece(game);
        if (piece == null)
        {
            moves.Add((forward.Rank == maxRank) switch
            {
                true => new Promotion(this, forward),
                false => new MoveOnly(this, forward)
            });

            if (!this.HasMoved)
            {
                var doubleMove = forward.Forward(game, this.IsWhite);
                if (doubleMove?.Piece(game) == null)
                    moves.Add(new DoubleMove(this, doubleMove!));
            }
        }

        if (forward.Left(game) is BoardSpace left)
        {
            if (left.Piece(game) is BoardPiece leftPiece)
            {
                if (leftPiece.IsWhite)
                    moves.Add((left.Rank == maxRank) switch
                    {
                        true => new PromotionCapture(this, leftPiece),
                        false => new Capture(this, leftPiece)
                    });
            }
            else if (left.Backward(game, this.IsWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, left, captured));
            }
        }

        if (forward.Right(game) is BoardSpace right)
        {
            if (right.Piece(game) is BoardPiece rightPiece)
            {
                if (rightPiece.IsWhite)
                    moves.Add((right.Rank == maxRank) switch
                    {
                        true => new PromotionCapture(this, rightPiece),
                        false => new Capture(this, rightPiece)
                    });
            }
            else if (right.Backward(game, this.IsWhite) is BoardSpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(game) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, right, captured));
            }
        }

        return moves;
    }

    /// <summary> handles promoting a white pawn to a black queen/knight/rook/bishop </summary>
    public override void Promote(ChessGame game, Promotion move)
    {
        if (move.PromoteTo is not SerializedType promoteTo) throw new Exception("Failed to promote to type");

        BoardPiece Piece;
        if (promoteTo == typeof(BlackQueen))
        {
            Piece = new BlackQueen(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.black_queen);
        }

        else if (promoteTo == typeof(BlackKnight))
        {
            Piece = new BlackKnight(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.black_knight);
        }

        else if (promoteTo == typeof(BlackBishop))
        {
            Piece = new BlackBishop(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.black_bishop);
        }

        else if (promoteTo == typeof(BlackRook))
        {
            Piece = new BlackRook(this.Id, this.Index.count, move.Destination, this.Index.prefix);
            Piece.PieceView!.SetIconResource(Resource.Drawable.black_rook);
        }
        else throw new Exception("Failed to promote to type");
        game.Player!.Pieces.Remove(this.Index);
        game.AllPieces.Remove(this.Index);
        game.Player!.Pieces[this.Index] = Piece;
        game.AllPieces[this.Index] = Piece;
        base.Promote(game, move);
        if (move is PromotionCapture capture)
            Piece.Capture(capture.Piece, game);
    }
}
