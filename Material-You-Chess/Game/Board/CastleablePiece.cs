using Chess.Game.Moves;

namespace Chess.Game.Board;

public class CastleablePiece(int id, BoardSpace space) : SpecialPiece(id, space)
{
    public override List<Move> Moves(ChessGame game)
    {
        var moves = base.Moves(game);
        if (game.Player!.King!.HasMoved)
            return moves;

        var rank = game.Player!.King!.Space.Rank;


        if (!game.Player!.Rook1!.HasMoved)
        {
            List<BoardSpace> queenSideSpaces = [];
            for (char queenSide = 'A'; queenSide != game.Player!.King!.Space.File; queenSide++)
                queenSideSpaces.Add(game.Board[(queenSide, rank)]);

            bool inDanger = false;
            foreach (var space in queenSideSpaces)
                if (game.IsInCheck(space))
                    inDanger = true;

            if (!inDanger)
                moves.Add(new QueenSideCastle());
        }



        if (!game.Player!.Rook2!.HasMoved)
        {
            List<BoardSpace> kingSideSpaces = [];
            for (char kingSide = 'H'; kingSide != game.Player!.King!.Space.File; kingSide++)
                kingSideSpaces.Add(game.Board[(kingSide, rank)]);

            bool inDanger = false;
            foreach (var space in kingSideSpaces)
                if (game.IsInCheck(space))
                    inDanger = true;

            if (!inDanger)
                moves.Add(new KingSideCastle());
        }

        return moves;
    }
}
