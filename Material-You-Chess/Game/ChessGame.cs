using System.Text.Json;
using Android.Gms.Nearby.Connection;
using Chess.App;
using Chess.App.Common;
using Chess.App.Networked;
using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;

namespace Chess.Game;

public class ChessGame : IChessGame
{
    public static ChessGame? Instance { get; set; }

    private int Turn = 1;
    private bool CurrentPlayerIsWhite = true;
    private readonly bool? clientPlayerIsWhite;

    public Dictionary<(string, int), BoardPiece> AllPieces { get; } = [];
    public Dictionary<(char file, int rank), BoardSpace> Board { get; } = [];
    public Toast WinnerToast => Toast.MakeText(this.Activity.Context, $"{this.Player!.Name} wins", ToastLength.Long)!;

    public IChessActivity Activity;
    public White? Player1 { get; set; }

    public Black? Player2 { get; set; }

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
                this.Moves = value.Moves(this);
        }
    }

    public List<Move>? Moves
    {
        get; set
        {
            if (field is not null)
                foreach (var move in field)
                    move.IndicateUnmovable();
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
            field?.IndicateUnmovable();
            field = value;

            //foreach (var space in this.Board.Values)
            //{
            //    space.Unselect();
            //}

            value?.IndicateUnmovable();
            value?.Select();
        }
    }

    public IPlayer? Player
    {
        get => this.CurrentPlayerIsWhite switch
        {
            true => this.Player1,
            false => this.Player2,
        };
    }

    public IPlayer? Enemy
    {
        get => !this.CurrentPlayerIsWhite switch
        {
            true => this.Player1,
            false => this.Player2
        };
    }

    public BoardSpace BindSpace(int id, int rank, char file)
    {
        const string isWhite = "IsWhite";
        const string isBlack = "IsBlack";
        var space = this.Activity.BoardLayout!.FindViewById<ImageView>(id);
        string? tag = (space?.Tag as Java.Lang.String)?.ToString();

        return new BoardSpace(file, rank, tag switch
        {
            isWhite => true,
            isBlack => false,
            _ => throw new Exception($"{this.Activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
        }, space!);
    }

    public ChessGame(IChessActivity activity, bool? clientPlayerIsWhite = null)
    {
        Instance = this;
        this.Activity = activity;
        this.clientPlayerIsWhite = clientPlayerIsWhite;

        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);

        foreach (var keyValuePair in this.Board)
        {
            keyValuePair.Value.SpaceView!.Click += OnClick;
            keyValuePair.Value.SpaceView!.Tag = new Java.Lang.String($"{keyValuePair.Key.file}{keyValuePair.Key.rank}");
            keyValuePair.Value.SpaceView!.Clickable = true;
        }

        this.Player1 = new White(activity.Player1Name!, this.Board, activity.PromotionDialogs.White);
        this.Player2 = new Black(activity.Player2Name!, this.Board, activity.PromotionDialogs.Black);

        this.AllPieces.Merge(this.Player1.Pieces, this.Player2.Pieces);

        foreach (var keyValuePair in this.AllPieces)
        {
            keyValuePair.Value.PieceView!.Click += OnClick;
            keyValuePair.Value.PieceView!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.PieceView!.Clickable = true;
        }
    }

    public void NextTurn(Move move)
    {
        this.Selected = null;
        if (!this.CurrentPlayerIsWhite)
            this.Turn += 1;

        this.LastMove = move;
        foreach (var piece in this.Player!.Pieces.Values)
        {
            piece.Update(true);
        }

        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }

    /// <returns> true if an enemy piece can capture the given piece</returns>
    public bool IsInCheck(BoardPiece Piece)
        => this.Player!.Pieces.Values.FirstOrDefault(p => Piece.IsWhite == p.IsWhite) != null
        ? this.IsInCheck(Piece.Space)
        : false;

    /// <returns> true if an enemy piece can capture a piece that would be placed on the given space</returns>
    public bool IsInCheck(BoardSpace Space)
    {
        List<Move> moves = [];
        foreach (var piece in this.Enemy!.Pieces.Values)
            moves.AddRange(piece.Moves(this));

        moves = [.. moves.Where(move => move is not MoveOnly)];
        return moves.FirstOrDefault(move => move.Destination == Space) != null;
    }

    private void OnClick(object? sender, EventArgs args)
    {
        if (sender is not ImageView imageView)
            return;

        if (imageView?.Tag is not Java.Lang.String javaString)
            return;

        if (this.clientPlayerIsWhite != null)
            if (this.clientPlayerIsWhite != this.CurrentPlayerIsWhite)
                return;

        this.Validate(javaString, out var pIndex, out var sIndex);

        if (this.Player!.Pieces.TryGetValue(pIndex, out BoardPiece? Piece))
        {
            if (this.Selected == null)
            {
                this.Selected = Piece;
                return;
            }

            if (this.Selected.IsWhite == Piece.IsWhite)
            {
                if (this.Selected.Id != Piece.Id)
                    this.Selected = Piece;

                return;
            }
        }

        if (!this.Board.TryGetValue(sIndex, out var space))
            return;

        if (this.IsCastling(space) is Move castling)
        {
            this.Activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: castling,
                SourceJsonGenerationContext.Default.Move)));
            this.Selected!.Move(castling, this);
            return;
        }

        var move = this.Moves?.FirstOrDefault(move => move.Destination.Index == space.Index);
        if (move is null)
        {
            this.Selected = null;
            return;
        }

        this.Activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: move, SourceJsonGenerationContext.Default.Move)));
        this.Selected!.Move(move, this);
    }

    private Move? IsCastling(BoardSpace space)
    {
        var rank = this.Player!.King!.Space.Rank;
        var queenSideMove = this.Moves?.FirstOrDefault(_move => _move is QueenSideCastle) as QueenSideCastle;
        var kingSideMove = this.Moves?.FirstOrDefault(_move => _move is KingSideCastle) as KingSideCastle;
        if (queenSideMove is not null)
        {
            List<BoardSpace> queenSideSpaces = [];
            for (char queenSide = 'A'; queenSide != this.Player!.King!.Space.File - 1; queenSide++)
                queenSideSpaces.Add(this.Board[(queenSide, rank)]);

            if (queenSideSpaces.FirstOrDefault(Space => Space.Index == space.Index) is not null)
                return queenSideMove;
        }

        if (kingSideMove is not null)
        {
            List<BoardSpace> kingSideSpaces = [];
            for (char kingSide = 'H'; kingSide != this.Player!.King!.Space.File + 1; kingSide++)
                kingSideSpaces.Add(this.Board[(kingSide, rank)]);

            if (kingSideSpaces.FirstOrDefault(Space => Space.Index == space.Index) is not null)
                return kingSideMove;
        }

        return null;
    }

    private bool Validate(Java.Lang.String Tag, out (string, int) pIndex, out (char, int) sIndex)
    {
        string tag = Tag.ToString();
        sIndex = (tag[0], int.Parse($"{tag[^1]}"));
        pIndex = (tag[0..^1], int.Parse($"{tag[^1]}"));
        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len <= 2|   unknown |   unknown | uppercase &   len > 2 

        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length <= 2) || (char.IsUpper(sIndex.Item1) && pIndex.Item1.Length > 2))
            return false;

        if (this.Player == null || this.Enemy == null)
            return false;


        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2))
        {
            if (!this.AllPieces.TryGetValue(pIndex, out BoardPiece? value))
                return false;

            sIndex = value.Space.Index;
        }

        return true;
    }
}
