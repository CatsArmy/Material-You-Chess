using System.Text.Json;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using Chess.App;
using Chess.App.Common;
using Chess.App.Networked;
using Chess.Dialogs;
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
    public Dictionary<(char, int), BoardSpace> Board { get; } = [];

    public Move? LastMove { get; set; }

    public IChessActivity Activity;

    public IPlayer? White { get; set; }

    public IPlayer? Black { get; set; }

    public BoardPiece? Selected
    {
        get; set
        {
            field = value;
            if (value is null)
                this.Moves = null;

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
                    move.Unselect();
            field = value;
            if (value is null)
                return;

            foreach (var move in value)
                move.Select();
        }
    }

    public Toast WinnerToast => Toast.MakeText(this.Activity.Context, $"{Player!.Name} wins", ToastLength.Long)!;

    public IPlayer? Player
    {
        get => this.CurrentPlayerIsWhite switch
        {
            true => this.White,
            false => this.Black,
        };
    }

    public IPlayer? Enemy
    {
        get => !this.CurrentPlayerIsWhite switch
        {
            true => this.White,
            false => this.Black
        };
    }

    private IPromotionDialog PromotionDialog
    {
        get => this.CurrentPlayerIsWhite switch
        {
            true => this.Activity.PromotionDialogs.White,
            false => this.Activity.PromotionDialogs.Black
        };
    }

    public ChessGame(IChessActivity activity, bool? clientPlayerIsWhite)
    {
        Instance = this;
        this.Activity = activity;
        const string isWhite = "IsWhite";
        const string isBlack = "IsBlack";
        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
        {
            var space = activity.BoardLayout!.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }

        foreach (var keyValuePair in this.Board)
        {
            keyValuePair.Value.SpaceView!.Click += OnClick;
            keyValuePair.Value.SpaceView!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.SpaceView!.Clickable = true;
        }

        this.White = new White(activity.Player1Name!, this.Board, this.Activity.BoardLayout!);
        this.Black = new Black(activity.Player2Name!, this.Board, this.Activity.BoardLayout!);

        this.AllPieces.Merge(this.White.Pieces, this.Black.Pieces);

        foreach (var keyValuePair in this.AllPieces)
        {
            keyValuePair.Value.Piece!.Click += OnClick;
            keyValuePair.Value.Piece!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Piece!.Clickable = true;
        }

        this.clientPlayerIsWhite = clientPlayerIsWhite;
    }

    public void NextTurn(Move move)
    {
        this.Selected = null;
        if (!this.CurrentPlayerIsWhite)
            this.Turn += 1;
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

    public void RedrawGame()
    {
        this.Activity.BoardLayout?.LayoutTransition?.DisableTransitionType(LayoutTransitionType.Changing);
        foreach (var piece in this.AllPieces.Values)
            piece.Move(new(piece, piece.Space), this);
        this.LastMove?.Select();
        this.Selected?.Space?.Select();
        this.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
    }

    private void OnClick(object? sender, EventArgs args)
    {
        if (sender is not ImageView imageView)
            return;

        if (imageView?.Tag is not Java.Lang.String javaString)
            return;

        if (this.clientPlayerIsWhite != null && this.CurrentPlayerIsWhite != this.clientPlayerIsWhite)
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
                {
                    this.Selected = Piece;
                }
                return;
            }
        }

        if (!this.Board.TryGetValue(sIndex, out var space))
            return;

        if (this.Moves?.FirstOrDefault(move => move.Destination.Index == space.Index) is not Move move)
        {
            this.Selected = null;
            return;
        }

        //Prevent promoting to an unknown type of piece
        if (move is Promotion promotion && promotion.PromoteTo is null)
        {
            this.PromotionDialog.Show(this, promotion);
            return;
        }

        this.Activity.Send(Payload.FromBytes(
        JsonSerializer.SerializeToUtf8Bytes<Move>(value: move, SourceJsonGenerationContext.Default.Move)
        //JsonSerializer.SerializeToUtf8Bytes(move, SourceJsonGenerationContext.Default.GetTypeInfo(move.GetType())!)
            ));

        //this.Selected!.Move(move, this);
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
