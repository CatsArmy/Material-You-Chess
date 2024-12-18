namespace Chess.ChessBoard;

[Serializable]
public class Pawn : Piece
{
    public bool isFirstMove = true;
    public bool EnPassantCapturable = false;

    public Pawn(ImageView piece, int id, ImageView space, bool isWhite, int spaceId, Action callback) : base(piece, id, space, isWhite, spaceId, callback) { }

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

    public override List<Move> Moves(Dictionary<(char, int), BoardSpace> board, Dictionary<(string, int), Piece> pieces)
    {
        List<Move> moves = new List<Move>();
        BoardSpace move = this.Forward(board, this.isWhite);
        if (move == null)
            return moves;
        var piece = move.GetPiece(pieces);
        if (piece == null)
        {
            moves.Add(new(move));
            if (this.isFirstMove)
            {
                var move2 = move.Forward(board, this.isWhite);
                if (move2.GetPiece(pieces) == null)
                    moves.Add(new(move2, false, true));
            }
        }

        (BoardSpace left, BoardSpace right) = this.isWhite switch
        {
            true => (DiagonalUp(board, false), DiagonalUp(board, true)),
            false => (DiagonalDown(board, false), DiagonalDown(board, true)),
        };

        if (left != null && left.GetPiece(pieces) != null)
            if (left.GetPiece(pieces).isWhite != this.isWhite)
                moves.Add(new(left, true));

        if (right != null && right.GetPiece(pieces) != null)
            if (right.GetPiece(pieces).isWhite != this.isWhite)
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
