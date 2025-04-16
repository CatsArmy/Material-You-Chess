using Chess.Game.Board;
using Chess.Game.Common;
using Chess.Game.Dialogs;
using Chess.Game.Moves;

namespace Chess.Game.Player;

public class Black(ChessGame game, string name) : IPlayer
{
    public string Name => name;
    public required IPromotionDialog PromotionDialog { get; init; }
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

    public Black(ChessGame game) : this(game, game.BlackClient.Username)
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

        foreach (var pieceIndexPair in this.Pieces)
            this.BindPiece(piece: pieceIndexPair.Value, index: pieceIndexPair.Key);
    }

    /// <summary> sets the OnClick of the view of the this players piece and adds the piece to the dictionary of all pieces </summary>
    /// <param name="piece">the piece(value) that will be added</param>
    /// <param name="index">the key of the value(piece) that will be added</param>
    private void BindPiece(BoardPiece piece, (string Prefix, int Count) index)
    {
        game.AllPieces[index] = piece;
        piece.PieceView!.Tag = new Java.Lang.String($"{piece.Prefix}{piece.Count}");
        piece.PieceView!.Clickable = true;
    }
}
