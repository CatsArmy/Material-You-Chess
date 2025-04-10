using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.Game.Board;
using Chess.Game.Common;
using Chess.Game.Interfaces;
using Chess.Game.Moves;

namespace Chess.Game.Player;

public class Black(ChessGame game, IPromotionDialog promotionDialog, string username) : IPlayer
{
    public string Name { get; } = username;
    public IPromotionDialog PromotionDialog { get; } = promotionDialog;
    public GameOutcome? Outcome { get; set; }

    #region Board Pieces
    public Dictionary<(string Prefix, int Count), BoardPiece> Pieces { get; set; } = [];
    public List<Pawn> Pawns { get; set; } = [];
    public Rook? Rook1 { get; set; }
    public Knight? Knight1 { get; set; }
    public Bishop? Bishop1 { get; set; }
    public King? King { get; set; }
    public Queen? Queen { get; set; }
    public Bishop? Bishop2 { get; set; }
    public Knight? Knight2 { get; set; }
    public Rook? Rook2 { get; set; }
    #endregion

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

    public Black(ChessGame game, IPromotionDialog promotionDialog, UserClient client) : this(game, promotionDialog, client.Username)
    {
        const int rank = 8;
        char file = 'A';
        this.Rook1 = new BlackRook(Resource.Id.gmp__bRook1, count: 1, game.Board[(file, rank)]);
        this.Pieces[this.Rook1.Index] = this.Rook1;
        file++; //B

        this.Knight1 = new BlackKnight(Resource.Id.gmp__bKnight1, count: 1, game.Board[(file, rank)]);
        this.Pieces[this.Knight1.Index] = this.Knight1;
        file++; //C

        this.Bishop1 = new BlackBishop(Resource.Id.gmp__bBishop1, 1, game.Board[(file, rank)]);
        this.Pieces[this.Bishop1.Index] = this.Bishop1;
        file++; //D

        this.Queen = new BlackQueen(Resource.Id.gmp__bQueen1, 1, game.Board[(file, rank)]);
        this.Pieces[this.Queen.Index] = this.Queen;
        file++; //E

        this.King = new BlackKing(Resource.Id.gmp__bKing1, 1, game.Board[(file, rank)]);
        this.Pieces[this.King.Index] = this.King;
        file++; //F

        this.Bishop2 = new BlackBishop(Resource.Id.gmp__bBishop2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Bishop2.Index] = this.Bishop2;
        file++; //G

        this.Knight2 = new BlackKnight(Resource.Id.gmp__bKnight2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Knight2.Index] = this.Knight2;
        file++; //H

        this.Rook2 = new BlackRook(Resource.Id.gmp__bRook2, 2, game.Board[(file, rank)]);
        this.Pieces[this.Rook2.Index] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++)
        {
            this.Pawns.Add(new BlackPawn(Resource.Id.gmp__bPawn1 + i, i + 1, game.Board[(file, rank - 1)]));
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
