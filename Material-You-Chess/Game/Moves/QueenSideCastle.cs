namespace Chess.Game.Moves;

public class QueenSideCastle() : Castling(ChessGame.Instance!.Player!.King!, ChessGame.Instance!.Player.Rook1!.Space)
{
    public override MoveOnly Rook
        => new(ChessGame.Instance!.Player!.Rook1!, ChessGame.Instance!.Board[('D', ChessGame.Instance!.Player!.Rook1!.Space.Rank)]);

    public override MoveOnly King
        => new(ChessGame.Instance!.Player!.King!, ChessGame.Instance!.Board[('C', ChessGame.Instance!.Player!.King!.Space.Rank)]);
}
