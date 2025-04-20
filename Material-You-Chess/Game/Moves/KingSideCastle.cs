using Material.You.Chess.Game.Board;

namespace Material.You.Chess.Game.Moves;

public class KingSideCastle(ChessGame game) : Castling(game.Player!.King!, game.Board[('G', game.Player!.King!.Space.Rank)])
{
    public KingSideCastle() : this(ChessActivity.Instance!.Game) { }

    public override char File => 'F';
    public override Rook Rook { get; } = game.Player!.Rook2!;
    public override MoveOnly PlayRook => new(this.Rook, game.Board[(this.File, this.Rook.Space.Rank)]);
}
