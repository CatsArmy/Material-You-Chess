using System.Text.Json;
using Android.Animation;
using Android.Content;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Dialogs;
using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;

namespace Chess.Game;

public class ChessGame : IChessGame
{
    private int Turn = 1;
    private bool CurrentPlayerIsWhite = true;
    private readonly bool? clientPlayerIsWhite;
    private WhitePromotionDialog WhitePromotionDialog { get; set; }
    private BlackPromotionDialog BlackPromotionDialog { get; set; }
    private readonly ConstraintLayout boardLayout;
    private readonly Action<Payload>? send;
    private readonly Context context;

    public Dictionary<(string, int), IPiece> AllPieces { get; } = [];

    public Dictionary<(char, int), ISpace> Board { get; } = [];

    public IMove? LastMove { get; set; } = null;

    public IPlayer? White { get; set; }

    public IPlayer? Black { get; set; }

    public IPiece? Selected
    {
        get; set
        {
            field = value;
            if (value is null)
                this.Moves = null;

            if (value is not null)
                this.Moves = value.Moves(this.Board, this.AllPieces);
        }
    }

    public List<IMove>? Moves
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
    private IPromotionDialog PromotionDialog
    {
        get => this.CurrentPlayerIsWhite switch
        {
            true => this.WhitePromotionDialog,
            false => this.BlackPromotionDialog
        };
    }

    private IPlayer? Player
    {
        get => this.CurrentPlayerIsWhite switch
        {
            true => this.White,
            false => this.Black,
        };
    }

    private IPlayer? Enemy
    {
        get => !this.CurrentPlayerIsWhite switch
        {
            true => this.White,
            false => this.Black
        };
    }

    public ChessGame(Context context, string whiteName, string blackName, ConstraintLayout boardLayout,
        WhitePromotionDialog whitePromotionDialog, BlackPromotionDialog blackPromotionDialog, bool? clientPlayerIsWhite, Action<Payload>? send)
    {
        this.context = context;
        this.boardLayout = boardLayout;
        this.WhitePromotionDialog = whitePromotionDialog;
        this.BlackPromotionDialog = blackPromotionDialog;
        this.clientPlayerIsWhite = clientPlayerIsWhite;
        this.send = send;

        const string isWhite = "IsWhite";
        const string isBlack = "IsBlack";
        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
        {
            var space = boardLayout.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{boardLayout.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }

        foreach (var keyValuePair in this.Board)
        {
            keyValuePair.Value.Space!.Click += OnClick;
            keyValuePair.Value.Space!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Space!.Clickable = true;
        }

        this.White = new White(whiteName, this.Board, this.boardLayout);
        this.Black = new Black(blackName, this.Board, this.boardLayout);

        this.AllPieces.Merge(this.White.Pieces, this.Black.Pieces);

        foreach (var keyValuePair in this.AllPieces)
        {
            keyValuePair.Value.Piece!.Click += OnClick;
            keyValuePair.Value.Piece!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Piece!.Clickable = true;
        }

        this.clientPlayerIsWhite = clientPlayerIsWhite;
    }

    private void OnClick(object? sender, EventArgs args)
    {
        if (this.clientPlayerIsWhite != null)
        {
            if (this.CurrentPlayerIsWhite != this.clientPlayerIsWhite)
            {
                return;
            }
        }

        if (sender is not ImageView imageView)
            return;

        if (imageView?.Tag is not Java.Lang.String javaString)
            return;

        string tag = javaString.ToString();
        var sIndex = (tag[0], int.Parse($"{tag[^1]}"));
        var pIndex = (tag[0..^1], int.Parse($"{tag[^1]}"));
        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len <= 2|   unknown |   unknown | uppercase &   len > 2 

        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length <= 2) || (char.IsUpper(sIndex.Item1) && pIndex.Item1.Length > 2))
            return;

        if (this.Player == null || this.Enemy == null)
            return;

        foreach (var piece in Player.Pieces.Values)
        {
            piece.Update();
        }

        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2))
        {
            if (!this.AllPieces.TryGetValue(pIndex, out IPiece? value))
                return;

            sIndex = value.Space.Index;
        }

        if (this.Player.Pieces.TryGetValue(pIndex, out IPiece? Piece))
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

        if (this.Moves?.FirstOrDefault(move => move.Destination.Index == space.Index) is not IMove move)
        {
            this.Selected = null;
            return;
        }

        if (move is Promote || move is PromoteAndCapture)
        {
            this.PromotionDialog.Show(this, (move as IPromote)!);
            return;
        }

        if (this.send != null)
        {
            var encodedMove = JsonSerializer.SerializeToUtf8Bytes(move);
            this.send(Payload.FromBytes(encodedMove));
        }
        this.OnMove(move);
        this.Selected!.Move(move.Destination);
        this.NextTurn();
    }

    public void OnMove(IMove move)
    {
        if (this.Player == null || this.Enemy == null)
            return;
        this.boardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);

        if (this.Selected is ISpecialBoardPiece piece)
        {
            piece.HasMoved = true;
        }

        if (move is DoubleMove pawn)
        {
            pawn.Pawn.EnPassantCapturable = true;
        }

        if (move is IPromote)
        {
            var Piece = this.Player.Pieces[move.OriginPiece.Index];
            if (move is PromoteQueen || move is PromoteQueenAndCapture)
            {
                Piece = new Queen(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, this.boardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.queen_white,
                    false => Resource.Drawable.queen_black
                });
            }

            else if (move is PromoteKnight || move is NetworkedPromoteKnightAndCapture)
            {
                Piece = new Knight(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, this.boardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.knight_white,
                    false => Resource.Drawable.knight_black
                });
            }

            else if (move is PromoteRook || move is PromoteRookAndCapture)
            {
                Piece = new Rook(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, this.boardLayout!) { HasMoved = true };
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.rook_white,
                    false => Resource.Drawable.rook_black
                });
            }

            else if (move is PromoteBishop || move is PromoteBishopAndCapture)
            {
                Piece = new Bishop(Piece.Id, Piece.Index, Piece.IsWhite, move.Destination, this.boardLayout!);
                Piece.Piece!.SetImageResource(Piece.IsWhite switch
                {
                    true => Resource.Drawable.bishop_white,
                    false => Resource.Drawable.bishop_black
                });
            }

            this.Player.Pieces[move.OriginPiece.Index] = Piece;
            this.AllPieces[move.OriginPiece.Index] = Piece;
        }

        if (move is not ICapture capture)
        {
            return;
        }

        this.OnCapture(capture);
    }
    public void NextTurn()
    {
        this.Selected = null;
        if (!this.CurrentPlayerIsWhite)
            this.Turn += 1;
        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }

    public void OnCapture(ICapture capture)
    {
        if (capture is EnPassant move)
        {
            this.Selected!.Capture(move.Pawn, this.AllPieces);
            return;
        }

        if (capture.Piece is King)
        {
            this.OnCaptureKing();
        }

        this.Selected!.Capture(capture.Piece, this.AllPieces);
    }

    public void OnCaptureKing()
    {
        this.Player!.Outcome = GameOutcome.Win;
        this.Enemy!.Outcome = GameOutcome.Lose;
        foreach (var Space in this.Board.Values)
            Space.Space!.Clickable = false;

        foreach (var piece in this.AllPieces.Values)
            piece.Space.Space!.Clickable = false;

        //display and handle the end of the game
        Toast.MakeText(context, $"{Player.Name} wins", ToastLength.Long);
    }

    /// <returns> true if an enemy piece can capture the given piece</returns>
    public bool IsInCheck(IPiece Piece)
    {
        foreach (var piece in this.Enemy!.Pieces.Values)
        {
            foreach (var move in piece.Moves(this.Board, this.AllPieces))
            {
                if (move.Destination == Piece.Space)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <returns> true if an enemy piece can capture a piece that would be placed on the given space</returns>
    public bool IsInCheck(ISpace Space)
    {
        foreach (var piece in this.Enemy!.Pieces.Values)
        {
            foreach (var move in piece.Moves(this.Board, this.AllPieces))
            {
                if (move is Pawn.SpecialMove)
                    continue;

                if (move.Destination == Space)
                    return true;
            }
        }

        return false;
    }
}
