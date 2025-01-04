using AndroidX.ConstraintLayout.Widget;

namespace Chess.ChessBoard;

/*[DataContract]*/
public class King(int id, (string, int) index, bool isWhite, ISpace space, ConstraintLayout boardLayout) : BoardPiece(id, index, abbreviation, isWhite, space, boardLayout)
{
    private const char abbreviation = 'K';
    /*[DataMember]*/
    public bool HasMoved = false;

    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);

        ISpace? up = this.Space?.Up(board);
        if (up != null)
        {
            if (up.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, up));
            else if (up.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? down = this.Space?.Down(board);
        if (down != null)
        {
            if (down.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, down));
            else if (down.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? left = this.Space?.Left(board);
        if (left != null)
        {
            if (left.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, left));
            else if (left.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? right = this.Space?.Right(board);
        if (right != null)
        {
            if (right.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, right));
            else if (right.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? topLeft = this.Space?.DiagonalUp(board, false);
        if (topLeft != null)
        {
            if (topLeft.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, topLeft));
            else if (topLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? topRight = this.Space?.DiagonalUp(board, true);
        if (topRight != null)
        {
            if (topRight.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, topRight));
            else if (topRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? bottomLeft = this.Space?.DiagonalDown(board, false);
        if (bottomLeft != null)
        {
            if (bottomLeft.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, bottomLeft));
            else if (bottomLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        ISpace? bottomRight = this.Space?.DiagonalDown(board, true);
        if (bottomRight != null)
        {
            if (bottomRight.Piece(pieces) is not IPiece piece)
                moves.Add(new Move(this, bottomRight));
            else if (bottomRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new Capture(this, piece));
        }

        return moves;
    }

    public bool IsInCheck(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        char key = this.IsWhite ? 'b' : 'w';
        List<IMove> moves = [];
        foreach (var piece in pieces)
        {
            if (!piece.Key.Item1.StartsWith(key))
                continue;

            moves.AddRange(piece.Value.Moves(board, pieces));
        }
        moves = [.. moves.Where(move => move is ICapture)];
        return moves.FirstOrDefault(move => IsCheck(move, pieces)) != null;
    }

    public bool IsCheck(IMove move, Dictionary<(string, int), IPiece> pieces)
    {
        var piece = move.Destination.Piece(pieces);
        if (piece == null)
            return false;
        return piece == this;
    }
}