using Chess.App;
using Chess.Game.Board;

namespace Chess.Game.Moves;

public class QueenSideCastle(ChessGame game) : Castling(game.Player!.King!, game.Board[('C', game.Player!.King!.Space.Rank)])
{
    public QueenSideCastle() : this(IChessActivity.Instance!.Game) { }

    public override char File => 'D';
    public override Rook Rook { get; } = game.Player!.Rook1!;
    public override MoveOnly PlayRook => new(this.Rook, game.Board[(this.File, this.Rook.Space.Rank)]);
}
