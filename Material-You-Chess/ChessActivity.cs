﻿using Android.Content.PM;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;
using Chess.Util.Logger;
using Firebase.Auth;
using Google.Android.Material.ImageView;
using Microsoft.Maui.ApplicationModel;

namespace Chess;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
    public static ChessActivity? Instance = null;
    public bool MaterialYouThemePreference;
    private AppPermissions? permissions;
    private ShapeableImageView? p1MainProfileImageView;
    private ShapeableImageView? p2MainProfileImageView;
    private TextView? p1MainUsername;
    private TextView? p2MainUsername;

    private Dictionary<(string, int), IPiece> pieces = new Dictionary<(string, int), IPiece>();
    private Dictionary<(char, int), ISpace> board = new Dictionary<(char, int), ISpace>();
    private IPiece? selected = null;
    private List<ISpace>? highlighted = null;
    private List<Move>? moves = null;

    private Bishop? bBishop1, bBishop2;
    private King? bKing;
    private Knight? bKnight1, bKnight2;
    private Pawn[] bPawns = new Pawn[8];
    private Queen? bQueen;
    private Rook? bRook1, bRook2;

    private Bishop? wBishop1, wBishop2;
    private King? wKing;
    private Knight? wKnight1, wKnight2;
    private Pawn[] wPawns = new Pawn[8];
    private Queen? wQueen;
    private Rook? wRook1, wRook2;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent?.GetStringExtra(nameof(this.MaterialYouThemePreference)), out this.MaterialYouThemePreference);
        if (hasValue && !this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.AppTheme_Material3_DayNight_NoActionBar);

        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        // Permission request logic
        this.permissions = new AppPermissions(this);

        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);

        //Run our logic
        this.p1MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        this.p1MainProfileImageView?.SetImageURI(FirebaseAuth.Instance?.CurrentUser?.PhotoUrl);
        this.p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        this.p2MainUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);

        this.p1MainUsername.Text = FirebaseAuth.Instance?.CurrentUser?.DisplayName;
        Instance = this;

        this.InitChessBoard();
        this.InitChessPieces();

        this.selected = null;
        this.highlighted = new();
        this.moves = new();
        //TODO implement En Passant difficulty Medium--
        //TODO Add Casstling difficulty Medium
        //TODO Add Promotion difficulty Easy+ / Medium-
        //TODO More?
        foreach (var space in this.board.Values)
        {
            space.Space.Click += (sender, e) => OnClickSpace(sender, e, space);
        }

        foreach (var piece in this.pieces.Values)
        {
            piece.Piece.Click += (sender, e) => OnClickPiece(sender, e, piece);
            piece.Piece.Clickable = true;
        }
        //The sequence "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"
        //describes the piece placement field of the starting position of a game of chess.
    }

    private void OnClickPiece(object? sender, EventArgs args, IPiece piece)
    {
        this.p1MainUsername.Text = $"{piece}";
        this.p2MainUsername.Text = $"{nameof(piece)}.{nameof(piece.IsWhite)}:{piece.IsWhite}";

        if (this.selected == null)
        {
            this.selected = piece;
            this.SelectMoves();
            return;
        }

        if (this.selected.IsWhite == piece.IsWhite)
        {
            if (this.selected.Id != piece.Id)
            {
                this.selected = piece;
                this.SelectMoves();
            }
            return;
        }

        var moves = this.selected.Moves(this.board, this.pieces);
        Log.Debug($"moves lookup done");
        var move = moves.FirstOrDefault(move => move.Destination == this.board[piece.Space.Index]);
        if (move == null)
        {
            ClearSelectedMoves();
            this.selected = null;
            return;
        }

        var (spaceId, spaceView) = this.selected.FakeMove(piece.Space.Id, piece.Space?.Space);
        var king = this.selected.IsWhite ? this.wKing : this.bKing;
        if (king.IsInCheck(board, pieces))
        {
            this.selected.FakeMove(spaceId, spaceView);
            Toast.MakeText(this, "Cant play that move would result in a check", ToastLength.Long)?.Show();
            return;
        }

        if (selected is Pawn pawn)
        {
            //pawn.isFirstMove = false;
            pawn.EnPassantCapturable = move.EnPassantCapturable;
            if (piece.Space?.File == (pawn.IsWhite ? 'H' : 'A'))
                /*await*/
                pawn.Promote();
        }
        /**/
        SelectMoves(piece);
        this.selected.Capture(piece!, pieces);

        this.NextPlayer();
    }

    private void OnClickSpace(object? sender, System.EventArgs e, ISpace space)
    {
        p1MainUsername.Text = $"{space}";
        p2MainUsername.Text = $"{nameof(space.IsWhite)}:{space.IsWhite}";

        if (this.selected == null)
            return;

        Move? move = this.selected?.Moves(this.board, this.pieces)?.FirstOrDefault(move => move.Destination == space);
        if (move == null)
        {
            ClearSelectedMoves();
            selected = null;
            return;
        }

        var layout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        layout?.LayoutTransition?.EnableTransitionType(Android.Animation.LayoutTransitionType.Changing);
        var (spaceId, spaceView) = this.selected!.FakeMove(space.Id, space.Space);
        var king = this.selected.IsWhite ? this.wKing : this.bKing;
        if (king.IsInCheck(this.board, this.pieces))
        {
            this.selected.FakeMove(spaceId, spaceView);
            Toast.MakeText(this, "Cant play that move would result in a check", ToastLength.Long)?.Show();
            return;
        }

        if (this.selected is Pawn pawn)
        {
            //pawn.isFirstMove = false;
            pawn.EnPassantCapturable = move.EnPassantCapturable;
            if (space.File == (pawn.IsWhite ? 'H' : 'A'))
                pawn.Promote();
        }

        this.SelectMoves(space);
        this.selected.Move(space);
        this.NextPlayer();
        return;
    }



    private void ClearSelectedMoves()
    {
        this.moves?.Clear();
        foreach (var space in this.highlighted!)
            space.UnselectSpace();
    }

    private void SelectMoves()
    {
        this.ClearSelectedMoves();
        this.moves = this.selected?.Moves(board, pieces);
        foreach (var move in this.moves!)
        {
            var space = this.board[move.Destination.Index]!;
            space.SelectSpace();
            this.highlighted?.Add(space);
        }
        var selected = this.board[this.selected!.Space.Index]!;
        selected.SelectSpace();
        this.highlighted!.Add(selected);
    }

    private void SelectMoves(IPiece piece) => SelectMoves(piece.Space);

    private void SelectMoves(ISpace space)
    {
        ClearSelectedMoves();
        space.SelectSpace();
        this.highlighted?.Add(space);

        var selected = this.selected!.Space;
        selected.SelectSpace();
        this.highlighted?.Add(selected);
    }

    private void NextPlayer()
    {
        this.selected = null;
        //Next player logic
    }

    private void OnNextPlayer()
    {
        //Update pieces and board
    }

    private void InitChessPieces()
    {
        char file = 'A';
        int blackRank = 8;
        int whiteRank = 1;
        this.bRook1 = new Rook(Resource.Id.gmp__bRook1, false, this.board[(file, blackRank)]);
        this.pieces[("bRook", 1)] = this.bRook1;

        this.wRook1 = new Rook(Resource.Id.gmp__wRook1, true, this.board[(file, whiteRank)]);
        this.pieces[("wRook", 1)] = this.wRook1;
        file++;

        this.bKnight1 = new Knight(Resource.Id.gmp__bKnight1, false, this.board[(file, blackRank)]);
        this.pieces[("bKnight", 1)] = this.bKnight1;

        this.wKnight1 = new Knight(Resource.Id.gmp__wKnight1, true, this.board[(file, whiteRank)]);
        this.pieces[("wKnight", 1)] = this.wKnight1;
        file++;

        this.bBishop1 = new Bishop(Resource.Id.gmp__bBishop1, false, this.board[(file, blackRank)]);
        this.pieces[("bBishop", 1)] = this.bBishop1;

        this.wBishop1 = new Bishop(Resource.Id.gmp__wBishop1, true, this.board[(file, whiteRank)]);
        this.pieces[("wBishop", 1)] = this.wBishop1;
        file++;

        this.bQueen = new Queen(Resource.Id.gmp__bQueen1, false, this.board[(file, blackRank)]);
        this.pieces[("bQueen", 1)] = this.bQueen;

        this.wQueen = new Queen(Resource.Id.gmp__wQueen1, true, this.board[(file, whiteRank)]);
        this.pieces[("wQueen", 1)] = this.wQueen;
        file++;

        this.bKing = new King(Resource.Id.gmp__bKing1, false, this.board[(file, blackRank)]);
        this.pieces[("bKing", 1)] = this.bKing;

        this.wKing = new King(Resource.Id.gmp__wKing1, true, this.board[(file, whiteRank)]);
        this.pieces[("wKing", 1)] = this.wKing;
        file++;

        this.bBishop2 = new Bishop(Resource.Id.gmp__bBishop2, false, this.board[(file, blackRank)]);
        this.pieces[("bBishop", 2)] = this.bBishop2;

        this.wBishop2 = new Bishop(Resource.Id.gmp__wBishop2, true, this.board[(file, whiteRank)]);
        this.pieces[("wBishop", 2)] = this.wBishop2;
        file++;

        this.bKnight2 = new Knight(Resource.Id.gmp__bKnight2, false, this.board[(file, blackRank)]);
        this.pieces[("bKnight", 2)] = this.bKnight2;

        this.wKnight2 = new Knight(Resource.Id.gmp__wKnight2, true, this.board[(file, whiteRank)]);
        this.pieces[("wKnight", 2)] = this.wKnight2;
        file++;

        this.bRook2 = new Rook(Resource.Id.gmp__bRook2, false, this.board[(file, blackRank)]);
        this.pieces[("bRook", 2)] = this.bRook2;

        this.wRook2 = new Rook(Resource.Id.gmp__wRook2, true, this.board[(file, whiteRank)]);
        this.pieces[("wRook", 2)] = this.wRook2;

        file = 'A';
        blackRank--;
        whiteRank++;

        for (int i = 0; i < 8; i++)
        {
            this.bPawns[i] = new Pawn(Resource.Id.gmp__bPawn1 + i, false, this.board[(file, blackRank)]);
            this.pieces[("bPawn", i + 1)] = bPawns[i];

            this.wPawns[i] = new Pawn(Resource.Id.gmp__wPawn1 + i, true, this.board[(file, whiteRank)]);
            this.pieces[("wPawn", i + 1)] = this.wPawns[i];
            file++;
        }
    }

    private void InitChessBoard()
    {
        const string IsWhite = nameof(IsWhite);
        const string IsBlack = nameof(IsBlack);
        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
