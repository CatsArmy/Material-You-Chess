using Android.Animation;
using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;
using Chess.Util;

namespace Chess.Game;

public class ChessGame : IChessGame
{
    private int Turn = 1;
    private bool CurrentPlayerIsWhite = true;
    private readonly ConstraintLayout BoardLayout;
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> AllPieces { get; } = [];
    /*[IgnoreDataMember]*/
    public Dictionary<(char, int), ISpace> Board { get; } = [];
    /*[DataMember]*/
    public IMove? LastMove { get; set; } = null;
    /*[IgnoreDataMember]*/
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

    /*[DataMember]*/
    public IPlayer? Player1 { get; set; }
    /*[DataMember]*/
    public IPlayer? Player2 { get; set; }

    /*[DataMember]*/
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

    public ChessGame(ConstraintLayout? boardLayout)
    {
        if (boardLayout == null)
            return;

        this.BoardLayout = boardLayout;

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

        this.Player1 = new White(this.Board, this.BoardLayout);
        this.Player2 = new Black(this.Board, this.BoardLayout);

        this.AllPieces.Merge(this.Player1.Pieces, this.Player2.Pieces);

        foreach (var keyValuePair in this.AllPieces)
        {
            keyValuePair.Value.Piece!.Click += OnClick;
            keyValuePair.Value.Piece!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Piece!.Clickable = true;
        }
    }

    private void OnClick(object? sender, EventArgs args)
    {
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

        var player = this.CurrentPlayerIsWhite switch
        {
            true => this.Player1,
            false => this.Player2,
        };
        if (player == null)
            return;

        foreach (var piece in player.Pieces.Values)
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

        if (player.Pieces.TryGetValue(pIndex, out IPiece? Piece))
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

        if (this.Selected is Pawn pawn)
        {
            pawn.HasMoved = true;
        }

        if (move is Pawn.DoubleMove doubleMove)
        {
            doubleMove.Pawn.EnPassantCapturable = true;
        }

        else if (move is Pawn.EnPassant enPassant)
        {
            this.Selected!.Capture(enPassant.Pawn, this.AllPieces);
        }

        else if (move is ICapture capture)
        {
            if (capture.Piece is King)
            {
                player.Outcome = GameOutcome.Win;
                foreach (var Space in this.Board.Values)
                    Space.Space!.Click -= this.OnClick;

                foreach (var piece in this.AllPieces.Values)
                    piece.Space.Space!.Click -= this.OnClick;
            }

            this.Selected!.Capture(capture.Piece, this.AllPieces);
        }

        this.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);

        this.Selected!.Move(move.Destination);
        this.Selected = null;
        if (!this.CurrentPlayerIsWhite)
            this.Turn += 1;
        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }
}