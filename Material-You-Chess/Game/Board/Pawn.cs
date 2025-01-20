using AndroidX.ConstraintLayout.Widget;
using Chess.Game.Moves;
using Newtonsoft.Json;

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

        var piece = forward.Piece(pieces);
        if (piece == null)
        {
            moves.Add(new Move(this, forward));
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
                    moves.Add(new Move(this, left));
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
                    moves.Add(new Move(this, right));
            }
            else if (right.Backward(board, isWhite) is ISpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(pieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassant(this, right, captured));
            }
        }
        return moves;
    }

    public void Promote()
    {
        //Get name via id
        //create a (string,int) key with the name
        //remove the value from the dict with the key
        //Open Promote dialog/popup thingy
        //add back the value but as a the selected piece(cant be king)

        //update the abbreviation to match the new type;
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
    public interface INetworkedSpecialMoves : INetworkedMove
    {
        public (string, int) Pawn { get; }
    }
}


public class EnPassant(IPiece origin, ISpace destination, Pawn captured) : Pawn.ISpecialMove, ICapture
{
    public ISpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public Pawn Pawn { get; } = captured;

    public IPiece Piece { get; } = captured;

    public NetworkedEnPassant ToNetworked() => new(this.OriginPiece.Index, this.Origin.Index, this.Pawn.Index, this.Destination.Index);

}

public class DoubleMove(Pawn origin, ISpace destination) : Pawn.ISpecialMove
{
    public ISpace Destination { get; set; } = destination;

    public int DestinationId { get; set; } = destination.Id;

    public ISpace Origin { get; set; } = origin.Space;

    public IPiece OriginPiece { get; set; } = origin;

    public int OriginId { get; set; } = origin.Id;

    public Pawn Pawn { get; } = origin;

    public NetworkedDoubleMove ToNetworked() => new(this.Destination.Index, this.Origin.Index, this.OriginPiece.Index);
}

[JsonObject(MemberSerialization.OptOut)]
public class NetworkedEnPassant((string, int) originPiece, (char, int) origin, (string, int) captured, (char, int) destination) : Pawn.INetworkedSpecialMoves, INetworkedCapture
{
    public (string, int) Pawn => captured;
    public (string, int) Piece => captured;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;
    public (string, int) OriginPiece { get; set; } = originPiece;

    public IMove FromNetworked(IChessGame game) => new EnPassant(game.AllPieces[this.OriginPiece!], game.Board[this.Destination], (game.AllPieces[this.Pawn] as Pawn)!);
}

[JsonObject(MemberSerialization.OptOut)]
public class NetworkedDoubleMove((char, int) destination, (char, int) origin, (string, int) originPiece) : Pawn.INetworkedSpecialMoves
{
    public (string, int) Pawn => originPiece;
    public (char, int) Destination { get; set; } = destination;
    public (char, int) Origin { get; set; } = origin;

#pragma warning disable CS9124
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
    public (string, int) OriginPiece { get; set; } = originPiece;
    // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
#pragma warning restore CS9124 

    public IMove FromNetworked(IChessGame game) => new DoubleMove((game.AllPieces[this.Pawn] as Pawn)!, game.Board[this.Destination]);
}