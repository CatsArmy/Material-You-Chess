using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

/*[DataContract]*/
public class Pawn(int id, (string, int) index, bool isWhite, ISpace space, ConstraintLayout boardLayout) : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'P';
    /*[DataMember]*/
    public bool HasMoved = false;

    /*[DataMember]*/
    public bool EnPassantCapturable = false;

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

    private interface ISpecialMoves : IMove
    {
        /*[DataMember]*/
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

    public class EnPassant(IPiece origin, ISpace destination, Pawn captured) : ISpecialMoves, ICapture
    {
        /*[DataMember]*/
        public ISpace Destination { get; set; } = destination;
        /*[DataMember]*/
        public int DestinationId { get; set; } = destination.Id;
        /*[DataMember]*/
        public ISpace Origin { get; set; } = origin.Space;
        /*[DataMember]*/
        public IPiece OriginPiece { get; set; } = origin;
        /*[DataMember]*/
        public int OriginId { get; set; } = origin.Id;
        /*[DataMember]*/
        public Pawn Pawn { get; } = captured;
        /*[DataMember]*/
        public IPiece Piece { get; } = captured;
    }

    public class DoubleMove(Pawn origin, ISpace destination) : ISpecialMoves
    {
        /*[DataMember]*/
        public ISpace Destination { get; set; } = destination;
        /*[DataMember]*/
        public int DestinationId { get; set; } = destination.Id;
        /*[DataMember]*/
        public ISpace Origin { get; set; } = origin.Space;
        /*[DataMember]*/
        public IPiece OriginPiece { get; set; } = origin;
        /*[DataMember]*/
        public int OriginId { get; set; } = origin.Id;
        /*[DataMember]*/
        public Pawn Pawn { get; } = origin;
    }
}
