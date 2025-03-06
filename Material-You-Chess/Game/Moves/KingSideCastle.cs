namespace Chess.Game.Moves;

public class KingSideCastle() : Castling(ChessGame.Instance!.Player!.King!, ChessGame.Instance!.Player.Rook1!.Space)
{
    public override MoveOnly Rook
        => new(ChessGame.Instance!.Player!.Rook1!, ChessGame.Instance!.Board[('F', ChessGame.Instance!.Player!.Rook1!.Space.Rank)]);

    public override MoveOnly King
        => new(ChessGame.Instance!.Player!.King!, ChessGame.Instance!.Board[('G', ChessGame.Instance!.Player!.King!.Space.Rank)]);
}
