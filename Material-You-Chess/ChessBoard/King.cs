namespace Chess.ChessBoard;
[Serializable]
public class King(int id, bool isWhite, ISpace space) : BoardPiece(id, isWhite, space)
{
    public bool HasMoved = false;

    public override List<Move> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<Move> moves = base.Moves(board, pieces);
        ISpace? up = this.Space?.Up(board);
        if (up != null)
            if (up.Piece(pieces) == null)
                moves.Add(new(up));
            else if (up.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(up, true));

        ISpace? down = this.Space?.Down(board);
        if (down != null)
            if (down.Piece(pieces) == null)
                moves.Add(new(down));
            else if (down.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(down, true));
        ISpace? left = this.Space?.Left(board);
        if (left != null)
            if (left.Piece(pieces) == null)
                moves.Add(new(left));
            else if (left.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(left, true));

        ISpace? right = this.Space?.Right(board);
        if (right != null)
            if (right.Piece(pieces) == null)
                moves.Add(new(right));
            else if (right.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(right, true));

        ISpace? topLeft = this.Space?.DiagonalUp(board, false);
        if (topLeft != null)
            if (topLeft.Piece(pieces) == null)
                moves.Add(new(topLeft));
            else if (topLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(topLeft, true));

        ISpace? topRight = this.Space?.DiagonalUp(board, true);
        if (topRight != null)
            if (topRight.Piece(pieces) == null)
                moves.Add(new(topRight));
            else if (topRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(topRight, true));

        ISpace? bottomLeft = this.Space?.DiagonalDown(board, false);
        if (bottomLeft != null)
            if (bottomLeft.Piece(pieces) == null)
                moves.Add(new(bottomLeft));
            else if (bottomLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(bottomLeft, true));

        ISpace? bottomRight = this.Space?.DiagonalDown(board, true);
        if (bottomRight != null)
            if (bottomRight.Piece(pieces) == null)
                moves.Add(new(bottomRight));
            else if (bottomRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(bottomRight, true));

        return moves;
    }

    public bool IsInCheck(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        char key = this.IsWhite ? 'b' : 'w';
        List<Move> moves = new List<Move>();
        foreach (var piece in pieces)
        {
            if (!piece.Key.Item1.StartsWith(key))
                continue;

            moves.AddRange(piece.Value.Moves(board, pieces));
        }
        moves = moves.Where(move => move.Capture).ToList();
        return moves.FirstOrDefault(move => IsCheck(move, pieces)) != null;
    }

    public bool IsCheck(Move move, Dictionary<(string, int), IPiece> pieces)
    {
        var piece = move.Space.Piece(pieces);
        if (piece == null)
            return false;
        return piece == this;
    }
}