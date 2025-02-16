using System.Text.Json.Serialization;
using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;

namespace Chess.Game.Board;

public class Pawn(int id, (string, int) index, bool isWhite, ISpace space, ConstraintLayout boardLayout)
    : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout), ISpecialBoardPiece
{
    public bool HasMoved { get; set; } = false;
    public bool EnPassantCapturable = false;

    private const char abbreviation = 'P';

    public override void Update()
    {
        this.EnPassantCapturable = false;
        base.Update();
    }

    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        if (this.Space.Forward(board, this.IsWhite) is not ISpace forward)
            return moves;
        int rank = this.IsWhite switch
        {
            true => 8,
            false => 1
        };
        var piece = forward.Piece(pieces);
        if (piece == null)
        {
            moves.Add((forward.Rank == rank) switch
            {
                true => new Promote(this, forward),
                false => new Move(this, forward)
            });
            if (!this.HasMoved)
            {
                var forwardX2 = forward.Forward(board, this.IsWhite);
                if (forwardX2?.Piece(pieces) == null)
                    moves.Add(new DoubleMove(this, forwardX2!));
            }
        }

        if (forward.Left(board) is ISpace left)
        {
            if (left.Piece(pieces) is IPiece leftPiece)
            {
                if (leftPiece?.IsWhite != this.IsWhite)
                    moves.Add((left.Rank == rank) switch
                    {
                        true => new PromoteAndCapture(this, leftPiece!),
                        false => new Capture(this, leftPiece!)
                    });
            }
            else if (left.Backward(board, isWhite) is ISpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(pieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, left, captured));
            }
        }

        if (forward.Right(board) is ISpace right)
        {
            if (right.Piece(pieces) is IPiece rightPiece)
            {
                if (rightPiece?.IsWhite != this.IsWhite)
                    moves.Add((right.Rank == rank) switch
                    {
                        true => new PromoteAndCapture(this, rightPiece!),
                        false => new Capture(this, rightPiece!)
                    });
            }
            else if (right.Backward(board, isWhite) is ISpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(pieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, right, captured));
            }
        }
        return moves;
    }

    public interface ISpecialMove : IMove
    {
        public Pawn Pawn { get; }

        public new void Select()
        {
            this.Destination.SelectSpace();
            this.Origin.SelectSpace();
            this.Pawn.Space.SelectSpace();
        }

        public new void Unselect()
        {
            this.Destination.UnselectSpace();
            this.Origin.UnselectSpace();
            this.Pawn.Space.SelectSpace();
        }
    }

    public interface INetworkedSpecialMove : INetworkedMove
    {
        public (string, int) Pawn { get; }
    }

    public class SpecialMove(Pawn origin, ISpace destination) : Pawn.ISpecialMove
    {
        public string Type { get; } = nameof(SpecialMove);
        public ISpace Destination { get; set; } = destination;

        public int DestinationId { get; set; } = destination.Id;

        public ISpace Origin { get; set; } = origin.Space;

        public IPiece OriginPiece { get; set; } = origin;

        public int OriginId { get; set; } = origin.Id;

        public Pawn Pawn { get; } = origin;


        public NetworkedSpecialMove ToNetworked() => new(this.Destination.Index, this.Origin.Index, this.OriginPiece.Index);
    }


    public class NetworkedSpecialMove((char, int) destination, (char, int) origin, (string, int) originPiece) : Pawn.INetworkedSpecialMove
    {
        public (string, int) Pawn => originPiece;
        public (char, int) Destination { get; set; } = destination;
        public (char, int) Origin { get; set; } = origin;

#pragma warning disable CS9124
        // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
        public (string, int) OriginPiece { get; set; } = originPiece;
        // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
#pragma warning restore CS9124

        public IMove FromNetworked(IChessGame game) => new SpecialMove((game.AllPieces[this.Pawn] as Pawn)!, game.Board[this.Destination]);
    }
}
