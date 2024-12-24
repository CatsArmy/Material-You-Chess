using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class Pawn(int id, bool isWhite, ISpace space) : BoardPiece(id, isWhite, space)
{
    public bool HasMoved = true;
    public bool EnPassantCapturable = false;
    public override char Abbreviation { get => base.Abbreviation; set => base.Abbreviation = value; }


    public override void Update()
    {
        if (this.HasMoved)
        {
            this.HasMoved = false;
        }
        else
        {
            this.EnPassantCapturable = false;
        }
    }

    public override List<Move> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<Move> moves = base.Moves(board, pieces);
        ISpace? move = this.Space.Forward(board, this.IsWhite);
        if (move == null)
            return moves;
        var piece = move.Piece(pieces);
        if (piece == null)
        {
            moves.Add(new(this.Space, move));
            if (this.HasMoved)
            {
                var move2 = move.Forward(board, this.IsWhite);
                if (move2?.Piece(pieces) == null)
                    moves.Add(new(this.Space, move2!, false, true));
            }
        }

        (ISpace? left, ISpace? right) = this.IsWhite switch
        {
            true => (this.Space.DiagonalUp(board, false), this.Space.DiagonalUp(board, true)),
            false => (this.Space.DiagonalDown(board, false), this.Space.DiagonalDown(board, true)),
        };

        if (left != null && left.Piece(pieces) != null)
            if (left.Piece(pieces).IsWhite != this.IsWhite)
                moves.Add(new(this.Space, left, true));

        if (right != null && right.Piece(pieces) != null)
            if (right.Piece(pieces).IsWhite != this.IsWhite)
                moves.Add(new(this.Space, right, true));

        return moves;
    }

    public void Promote()
    {
        //Get name via id
        //create a (string,int) key with the name
        //remove the value from the dict with the key
        //Open Promote dialog/popup thingy
        //add back the value but as a the selected piece(cant be king)
    }
}
