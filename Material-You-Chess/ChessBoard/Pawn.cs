namespace Chess.ChessBoard;

[Serializable]
public class Pawn(int id, bool isWhite, ISpace space) : BoardPiece(id, isWhite, space)
{
    public bool isFirstMove = true;
    public bool EnPassantCapturable = false;

    public override void Update()
    {
        if (this.isFirstMove)
        {
            this.isFirstMove = false;
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
            moves.Add(new(move));
            if (this.isFirstMove)
            {
                var move2 = move.Forward(board, this.IsWhite);
                if (move2?.Piece(pieces) == null)
                    moves.Add(new(move2!, false, true));
            }
        }

        (ISpace? left, ISpace? right) = this.IsWhite switch
        {
            true => (this.Space.DiagonalUp(board, false), this.Space.DiagonalUp(board, true)),
            false => (this.Space.DiagonalDown(board, false), this.Space.DiagonalDown(board, true)),
        };

        if (left != null && left.Piece(pieces) != null)
            if (left.Piece(pieces).IsWhite != this.IsWhite)
                moves.Add(new(left, true));

        if (right != null && right.Piece(pieces) != null)
            if (right.Piece(pieces).IsWhite != this.IsWhite)
                moves.Add(new(right, true));

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
