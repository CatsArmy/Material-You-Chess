using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class Pawn(int id, (string, int) index, bool isWhite, ISpace space) : BoardPiece(id, index, _Abbreviation, isWhite, space)
{
    private const char _Abbreviation = 'P';
    public bool HasMoved = false;
    public bool EnPassantCapturable = false;

    public override void Update()
    {
        this.EnPassantCapturable = false;
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
                    moves.Add(new DoubleMove(this, forwardX2!, delegate
                    {
                        this.HasMoved = true;
                        this.EnPassantCapturable = true;
                    }));
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
                    moves.Add(new EnPassantMove(this, left, captured));
            }
        }

        if (forward.Left(board) is ISpace right)
        {
            if (right.Piece(pieces) is IPiece rightPiece)
            {
                if (rightPiece?.IsWhite != this.IsWhite)
                    moves.Add(new Move(this, right));
            }
            else if (right.Backward(board, isWhite) is ISpace EnPassantSpace)
            {
                if (EnPassantSpace.Piece(pieces) is Pawn captured && captured.EnPassantCapturable)
                    moves.Add(new EnPassantMove(this, right, captured));
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

        ///update the abbriviation to match the new type;
    }

    public class DoubleMove(IPiece origin, ISpace destination) : IMove
    {
        public ISpace Destination { get; set; } = destination;
        public int DestinationId { get; set; } = destination.Id;
        public ISpace Origin { get; set; } = origin.Space;
        public IPiece OriginPiece { get; set; } = origin;
        public int OriginId { get; set; } = origin.Id;
    }
}
