using System.Runtime.Serialization;

namespace Chess.ChessBoard;

[DataContract]
public class King(int id, (string, int) index, bool isWhite, ISpace space) : BoardPiece(id, index, _Abbreviation, isWhite, space)
{
    private const char _Abbreviation = 'K';
    public bool HasMoved = false;

    public override List<IMove> Moves(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        List<IMove> moves = base.Moves(board, pieces);
        ISpace? up = this.Space?.Up(board);
        if (up != null)
            if (up.Piece(pieces) == null)
                moves.Add(new(this.Space, up));
            else if (up.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, up, true));

        ISpace? down = this.Space?.Down(board);
        if (down != null)
            if (down.Piece(pieces) == null)
                moves.Add(new(this.Space, down));
            else if (down.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, down, true));
        ISpace? left = this.Space?.Left(board);
        if (left != null)
            if (left.Piece(pieces) == null)
                moves.Add(new(this.Space, left));
            else if (left.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, left, true));

        ISpace? right = this.Space?.Right(board);
        if (right != null)
            if (right.Piece(pieces) == null)
                moves.Add(new(this.Space, right));
            else if (right.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, right, true));

        ISpace? topLeft = this.Space?.DiagonalUp(board, false);
        if (topLeft != null)
            if (topLeft.Piece(pieces) == null)
                moves.Add(new(this.Space, topLeft));
            else if (topLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, topLeft, true));

        ISpace? topRight = this.Space?.DiagonalUp(board, true);
        if (topRight != null)
            if (topRight.Piece(pieces) == null)
                moves.Add(new(this.Space, topRight));
            else if (topRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, topRight, true));

        ISpace? bottomLeft = this.Space?.DiagonalDown(board, false);
        if (bottomLeft != null)
            if (bottomLeft.Piece(pieces) == null)
                moves.Add(new(this.Space, bottomLeft));
            else if (bottomLeft.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, bottomLeft, true));

        ISpace? bottomRight = this.Space?.DiagonalDown(board, true);
        if (bottomRight != null)
            if (bottomRight.Piece(pieces) == null)
                moves.Add(new(this.Space, bottomRight));
            else if (bottomRight.Piece(pieces)?.IsWhite != this.IsWhite)
                moves.Add(new(this.Space, bottomRight, true));

        return moves;
    }

    public bool IsInCheck(Dictionary<(char, int), ISpace> board, Dictionary<(string, int), IPiece> pieces)
    {
        char key = this.IsWhite ? 'b' : 'w';
        List<IMove> moves = new List<IMove>();
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
        var piece = move.Destination.Piece(pieces);
        if (piece == null)
            return false;
        return piece == this;
    }
}