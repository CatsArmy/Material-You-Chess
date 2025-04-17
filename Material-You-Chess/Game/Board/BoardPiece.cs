using System.Text.Json.Serialization;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Game.Moves;
using Google.Android.Material.Button;

namespace Chess.Game.Board;

[JsonPolymorphic()]
[JsonDerivedType(typeof(BoardPiece), nameof(BoardPiece))]
[JsonDerivedType(typeof(SpecialPiece), nameof(SpecialPiece))]
[JsonDerivedType(typeof(Knight), nameof(Knight))]
[JsonDerivedType(typeof(Bishop), nameof(Bishop))]
[JsonDerivedType(typeof(Queen), nameof(Queen))]
[JsonDerivedType(typeof(King), nameof(King))]
[JsonDerivedType(typeof(Pawn), nameof(Pawn))]
[JsonDerivedType(typeof(Rook), nameof(Rook))]
[JsonDerivedType(typeof(WhiteKing), nameof(WhiteKing))]
[JsonDerivedType(typeof(WhitePawn), nameof(WhitePawn))]
[JsonDerivedType(typeof(WhiteRook), nameof(WhiteRook))]
[JsonDerivedType(typeof(WhiteQueen), nameof(WhiteQueen))]
[JsonDerivedType(typeof(WhiteBishop), nameof(WhiteBishop))]
[JsonDerivedType(typeof(WhiteKnight), nameof(WhiteKnight))]
[JsonDerivedType(typeof(BlackKnight), nameof(BlackKnight))]
[JsonDerivedType(typeof(BlackBishop), nameof(BlackBishop))]
[JsonDerivedType(typeof(BlackQueen), nameof(BlackQueen))]
[JsonDerivedType(typeof(BlackKing), nameof(BlackKing))]
[JsonDerivedType(typeof(BlackPawn), nameof(BlackPawn))]
[JsonDerivedType(typeof(BlackRook), nameof(BlackRook))]
public class BoardPiece(MaterialButton PieceView, BoardSpace space)
{
    private static readonly ConstraintLayout? BoardLayout = ChessActivity.Instance?.BoardLayout;
    public BoardPiece(int id, BoardSpace space) : this(BoardLayout!.FindViewById<MaterialButton>(id)!, space) { }

    public int Id { get; } = PieceView.Id;
    public BoardSpace Space { get; set; } = space;
    [JsonIgnore] public BoardSpace? LastSpace { get; set; }
    [JsonIgnore] public MaterialButton? PieceView { get; set; } = PieceView;
    public (string prefix, int count) Index => (this.Prefix, this.Count);
#nullable disable
    public virtual string Prefix { get; }
#nullable restore
    public virtual int Count { get; }
    public virtual bool IsWhite { get; }
    public virtual char Abbreviation { get; }

    /// <summary>An virtual method that the overrider will use to generate the available moves at this state of the game</summary>
    /// <returns>a list of available moves based on the rules of the chess game</returns>
    public virtual List<Move> Moves(ChessGame game) => [];

    /// <summary>this function plays the move and is overwritten to implement the special rules of chess </summary>
    public virtual void Move(Move move, ChessGame game)
    {
        if (move is Capture capture)
        {
            this.Capture(capture.Piece, game);
        }

        this.Move(move);
    }

    /// <remarks> Visually moves the piece </remarks>
    /// <summary> this is the BoardPiece that will be moved to the move </summary>
    public void Move(Move move)
    {
        this.LastSpace = this.Space;
        this.Space = move.Destination;
        if (this.PieceView?.Parent is not ConstraintLayout parent)
        {
            Logger.Warn("PieceView is not in a ConstraintLayout parent");
            return;
        }

        var constraintSet = new ConstraintSet();
        constraintSet.Clone(parent);

        int pieceViewId = this.PieceView.Id;
        int destinationId = this.Space.SpaceView!.Id;

        // Center the piece within the destination space
        constraintSet.Connect(pieceViewId, ConstraintSet.Top, destinationId, ConstraintSet.Top);
        constraintSet.Connect(pieceViewId, ConstraintSet.Bottom, destinationId, ConstraintSet.Bottom);
        constraintSet.Connect(pieceViewId, ConstraintSet.Start, destinationId, ConstraintSet.Start);
        constraintSet.Connect(pieceViewId, ConstraintSet.End, destinationId, ConstraintSet.End);

        // Apply the updated constraints
        constraintSet.ApplyTo(parent);
        this.PieceView.RequestLayout();
    }

    /// <remarks> 
    /// this is the BoardPiece that will Capture an enemy's BoardPiece 
    /// </remarks>
    /// <summary> this BoardPiece is the Move.Origin </summary>
    /// <param name="destination"> is the BoardPiece that will be Captured by the player's BoardPiece</param>
    public virtual void Capture(BoardPiece destination, ChessGame game) => destination.Capture(game);

    /// <summary> this BoardPiece is the Move.Destination </summary>
    /// <remarks> this is the BoardPiece that will be Captured </remarks>
    public virtual void Capture(ChessGame game)
    {
        game.AllPieces.Remove(this.Index);
        game.Player!.Pieces.Remove(this.Index);
        this.PieceView!.Enabled = false;
        this.PieceView!.Clickable = false;
        this.PieceView!.Visibility = Android.Views.ViewStates.Gone;
    }

    /// <summary>
    /// adds all the diagonal spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    public void Diagonals(ChessGame game, ref List<Move> moves)
    {
        this.DiagonalsUpRight(game, ref moves);
        this.DiagonalsUpLeft(game, ref moves);
        this.DiagonalsDownRight(game, ref moves);
        this.DiagonalsDownLeft(game, ref moves);
    }

    /// <summary>
    /// adds all the diagonal up right spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void DiagonalsUpRight(ChessGame game, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalUpRight(game); diagonal != null; diagonal = diagonal.DiagonalUpRight(game))
        {
            if (diagonal == null) break;

            if (diagonal.Piece(game) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    /// <summary>
    /// adds all the diagonal up left spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void DiagonalsUpLeft(ChessGame game, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalUpLeft(game); diagonal != null; diagonal = diagonal.DiagonalUpLeft(game))
        {
            if (diagonal == null) break;
            if (diagonal.Piece(game) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece is not null && diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    /// <summary>
    /// adds all the diagonal down right spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void DiagonalsDownRight(ChessGame game, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalDownRight(game); diagonal != null; diagonal = diagonal.DiagonalDownRight(game))
        {
            if (diagonal == null) break;
            if (diagonal.Piece(game) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    /// <summary>
    /// adds all the diagonal down left spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void DiagonalsDownLeft(ChessGame game, ref List<Move> moves)
    {
        for (var diagonal = this.Space.DiagonalDownLeft(game); diagonal != null; diagonal = diagonal.DiagonalDownLeft(game))
        {
            if (diagonal == null) break;
            if (diagonal.Piece(game) is BoardPiece diagonalPiece)
            {
                if (diagonalPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, diagonalPiece));
                break;
            }

            moves.Add(new Move(this, diagonal));
        }
    }

    /// <summary>
    /// adds all the horizontal spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    public void Horizontals(ChessGame game, ref List<Move> moves)
    {
        this.HorizontalsRight(game, ref moves);
        this.HorizontalsLeft(game, ref moves);
    }

    /// <summary>
    /// adds all the horizontal right spaces(Space.Right) that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void HorizontalsRight(ChessGame game, ref List<Move> moves)
    {
        for (var horizontal = this.Space.Right(game); horizontal != null; horizontal = horizontal.Right(game))
        {
            if (horizontal == null) break;
            if (horizontal.Piece(game) is BoardPiece rightPiece)
            {
                if (rightPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, rightPiece));
                break;
            }

            moves.Add(new Move(this, horizontal));
        }
    }

    /// <summary>
    /// adds all the horizontal left spaces(Space.left) that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void HorizontalsLeft(ChessGame game, ref List<Move> moves)
    {
        for (var horizontal = this.Space.Left(game); horizontal != null; horizontal = horizontal.Left(game))
        {
            if (horizontal == null) break;
            if (horizontal.Piece(game) is BoardPiece leftPiece)
            {
                if (leftPiece.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, leftPiece));
                break;
            }

            moves.Add(new Move(this, horizontal));
        }
    }

    /// <summary>
    /// adds all the vertical spaces that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    public void Verticals(ChessGame game, ref List<Move> moves)
    {
        this.VerticalsUp(game, ref moves);
        this.VerticalsDown(game, ref moves);
    }

    /// <summary>
    /// adds all the vertical up spaces(Space.Up) that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void VerticalsUp(ChessGame game, ref List<Move> moves)
    {
        for (var vertical = this.Space.Up(game); vertical != null; vertical = vertical.Up(game))
        {
            if (vertical == null) break;
            if (vertical.Piece(game) is BoardPiece pieceAbove)
            {
                if (pieceAbove.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, pieceAbove));
                break;
            }

            moves.Add(new Move(this, vertical));
        }
    }

    /// <summary>
    /// adds all the vertical down spaces(Space.Down) that this piece can move to
    /// be it a capture move or a regual potential capture move
    /// </summary>
    private void VerticalsDown(ChessGame game, ref List<Move> moves)
    {
        for (var vertical = this.Space.Down(game); vertical != null; vertical = vertical.Down(game))
        {
            if (vertical == null) break;
            if (vertical.Piece(game) is BoardPiece pieceBelow)
            {
                if (pieceBelow.IsWhite != this.IsWhite)
                    moves.Add(new Capture(this, pieceBelow));
                break;
            }

            moves.Add(new Move(this, vertical));
        }
    }
}