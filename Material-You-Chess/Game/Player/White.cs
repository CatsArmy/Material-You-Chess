using Chess.Game.Board;
using Chess.Game.Common;
using Chess.Game.Dialogs;
using Chess.Game.Moves;

namespace Chess.Game.Player;

public class White(ChessGame game, IPromotionDialog promotionDialog, string username) : IPlayer
{
    public string Name { get; } = username;
    public IPromotionDialog PromotionDialog { get; } = promotionDialog;
    public GameOutcome? Outcome { get; set; }

    #region Board Pieces
    public Dictionary<(string Prefix, int Count), BoardPiece> Pieces { get; } = [];
    public List<Pawn> Pawns { get; } = [];
    public Rook? Rook1 { get; set; }
    public Knight? Knight1 { get; set; }
    public Bishop? Bishop1 { get; set; }
    public King? King { get; set; }
    public Queen? Queen { get; set; }
    public Bishop? Bishop2 { get; set; }
    public Knight? Knight2 { get; set; }
    public Rook? Rook2 { get; set; }
    #endregion

    /// <summary> when setting the Selected BoardPiece it will also set the Moves to </summary>
    public BoardPiece? Selected
    {
        get; set
        {
            if (value is null)
            {
                this.Moves = null;
            }
            field = value;

            if (value is not null)
                this.Moves = value.Moves(game);
        }
    }

    /// <summary>
    /// when setting the Moves BoardSpace
    /// it may call any of the following methods accordingly
    /// Move.Select
    /// Move.Unselect
    /// Move.IndicateMoveable
    /// Move.UnindicateMoveable
    /// </summary>
    public List<Move>? Moves
    {
        get; set
        {
            if (field is not null)
                foreach (var move in field)
                    move.UnindicateMoveable();
            field = value;
            if (value is null)
                return;

            foreach (var move in value)
                move.IndicateMoveable();
        }
    }

    /// <summary> 
    /// When trying to set the value to null
    /// it will only clear the selected spaces and keep the last move for later use. 
    /// </summary> 
    /// <remarks> 
    /// LastMove will be null only at the start of the game when the player has not played any moves yet.
    /// </remarks>
    public Move? LastMove
    {
        get; set
        {
            field?.Unselect();
            field?.UnindicateMoveable();
            if (value is null)
                return;
            field = value;

            value?.UnindicateMoveable();
            value?.Select();
        }
    }

    public White(ChessGame game, IPromotionDialog promotionDialog, PlayerClient client) : this(game, promotionDialog, client.Username)
    {
        const int rank = 1;
        char file = 'A';
        this.Rook1 = new WhiteRook(Resource.Id.gmp__wRook1, count: 1, game.Board[(file, rank)]);
        this.Pieces[this.Rook1.Index] = this.Rook1;
        file++; //B

        this.Knight1 = new WhiteKnight(Resource.Id.gmp__wKnight1, count: 1, game.Board[(file, rank)]);
        this.Pieces[this.Knight1.Index] = this.Knight1;
        file++; //C

        this.Bishop1 = new WhiteBishop(Resource.Id.gmp__wBishop1, 1, game.Board[(file, rank)]);
        this.Pieces[this.Bishop1.Index] = this.Bishop1;
        file++; //D

        this.Queen = new WhiteQueen(Resource.Id.gmp__wQueen1, 1, game.Board[(file, rank)]);
        this.Pieces[this.Queen.Index] = this.Queen;
        file++; //E

        this.King = new WhiteKing(Resource.Id.gmp__wKing1, 1, game.Board[(file, rank)]);
        this.Pieces[this.King.Index] = this.King;
        file++; //F

        this.Bishop2 = new WhiteBishop(Resource.Id.gmp__wBishop2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Bishop2.Index] = this.Bishop2;
        file++; //G

        this.Knight2 = new WhiteKnight(Resource.Id.gmp__wKnight2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Knight2.Index] = this.Knight2;
        file++; //H

        this.Rook2 = new WhiteRook(Resource.Id.gmp__wRook2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Rook2.Index] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++)
        {
            this.Pawns.Add(new WhitePawn(Resource.Id.gmp__wPawn1 + i, i + 1, game.Board[(file, rank + 1)]));
            this.Pieces[this.Pawns[i].Index] = this.Pawns[i];
            file++;
        }

        foreach (var kvp in this.Pieces)
        {
            var piece = kvp.Value;
            var index = kvp.Key;
            game.AllPieces[index] = piece;
            piece.PieceView!.Tag = new Java.Lang.String($"{piece.Prefix}{piece.Count}");
            piece.PieceView!.Clickable = true;
        }
    }
}
