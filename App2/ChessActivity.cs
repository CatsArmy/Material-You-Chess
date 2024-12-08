﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;
using Google.Android.Material.ImageView;
using Space = Chess.ChessBoard.Space;

namespace Chess;


[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
    public bool MaterialYouThemePreference;
    private string PlayerName = "Guest2";
    private AppPermissions permissions;
    private Android.Net.Uri uri;

    private Dictionary<(string, int), Piece> pieces = new Dictionary<(string, int), Piece>();
    private Dictionary<(char, int), Space> board = new Dictionary<(char, int), Space>();
    private Piece selected = null;
    private List<Space> highlighted = null;
    private List<Move> moves = null;
    private readonly Color SelectedBlackColor = new(82, 70, 152);
    private readonly Color SelectedWhiteColor = new(172, 162, 225);
    private readonly Color UnselectedBlackColor = new(41, 43, 50);
    private readonly Color UnselectedWhiteColor = new(222, 227, 195);

    private Bishop bBishop1, bBishop2;
    private King bKing;
    private Knight bKnight1, bKnight2;
    private Pawn[] bPawns = new Pawn[8];
    private Queen bQueen;
    private Rook bRook1, bRook2;

    private Bishop wBishop1, wBishop2;
    private King wKing;
    private Knight wKnight1, wKnight2;
    private Pawn[] wPawns = new Pawn[8];
    private Queen wQueen;
    private Rook wRook1, wRook2;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        bool hasValue = bool.TryParse(base.Intent.GetStringExtra(nameof(this.MaterialYouThemePreference)), out this.MaterialYouThemePreference);
        if (hasValue && !this.MaterialYouThemePreference)
            base.SetTheme(Resource.Style.Theme_Material3_DayNight_NoActionBar_Alt);

        base.OnCreate(savedInstanceState);
        Xamarin.Essentials.Platform.Init(this, savedInstanceState);

        // Permission request logic
        this.permissions = new AppPermissions();
        this.permissions.RequestPermissions(this);


        //Set our view
        base.SetContentView(Resource.Layout.chess_activity);
        //Run our logic
        this.PlayerName = base.Intent.GetStringExtra(nameof(this.PlayerName));
        this.uri = Android.Net.Uri.Parse(base.Intent.GetStringExtra(nameof(uri)));

        ShapeableImageView p1MainProfileImageView = base.FindViewById<ShapeableImageView>(Resource.Id.p1MainProfileImageView);
        if (this.uri != null)
            p1MainProfileImageView.SetImageURI(this.uri);
        TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        p1MainUsername.Text = this.PlayerName;

        this.InitChessPieces();
        this.InitChessBoard();
        selected = null;
        highlighted = new();
        moves = new();
        //TODO implement EnPassantCapturable
        //TODO Add Casstling
        //TODO Add Promotion
        //TODO More?
        foreach (var space in board.Values)
        {
            space.space.Click += (sender, e) => OnClickSpace(sender, e, space);
        }

        foreach (var piece in pieces.Values)
        {
            piece.piece.Click += (sender, e) => OnClickPiece(sender, e, piece);
            piece.piece.Clickable = true;
        }
    }

    private void OnClickPiece(object sender, System.EventArgs e, Piece piece)
    {
        TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        p1MainUsername.Text = $"{piece}";
        TextView p2MainUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);
        p2MainUsername.Text = $"{nameof(piece)}.{nameof(piece.isWhite)}:{piece.isWhite}";

        if (selected == null)
        {
            selected = piece;
            SelectMoves();
            return;
        }

        if (selected.isWhite == piece.isWhite)
        {
            if (selected.id != piece.id)
            {
                selected = piece;
                SelectMoves();
            }
            return;
        }

        var move = selected.Moves(board, pieces).FirstOrDefault(move => move.Space == board[piece.GetBoardIndex()]);
        if (move == null)
        {
            ClearSelectedMoves();
            selected = null;
            return;
        }

        var (spaceId, spaceView) = selected.FakeMove(piece.spaceId, piece.space);
        var king = selected.isWhite ? wKing : bKing;
        if (king.IsInCheck(board, pieces))
        {
            selected.FakeMove(spaceId, spaceView);
            Toast.MakeText(this, "Cant play that move would result in a check", ToastLength.Long).Show();
            return;
        }

        if (selected is Pawn pawn)
        {
            //pawn.isFirstMove = false;
            pawn.EnPassantCapturable = move.EnPassantCapturable;
            if (piece.GetBoardIndex().Item1 == (pawn.isWhite ? 'H' : 'A'))
                pawn.Promote();
        }

        SelectMoves(piece);
        selected.Capture(piece, pieces);
        View view = base.FindViewById(piece.piece.Id);
        var parent = view.Parent as ViewGroup;
        parent?.RemoveView(view);
        this.NextPlayer();
    }

    private void OnClickSpace(object sender, System.EventArgs e, Space space)
    {
        TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        p1MainUsername.Text = $"{space}";
        TextView p2MainUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);
        p2MainUsername.Text = $"{nameof(space.isWhite)}:{space.isWhite}";
        if (selected == null)
            return;
        Move move = selected.Moves(board, pieces).FirstOrDefault(move => move.Space == space);
        if (move == null)
        {
            ClearSelectedMoves();
            selected = null;
            return;
        }

        var view = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        view.LayoutTransition.EnableTransitionType(Android.Animation.LayoutTransitionType.Changing);
        var (spaceId, spaceView) = selected.FakeMove(space.spaceId, space.space);
        var king = selected.isWhite ? wKing : bKing;
        if (king.IsInCheck(board, pieces))
        {
            selected.FakeMove(spaceId, spaceView);
            Toast.MakeText(this, "Cant play that move would result in a check", ToastLength.Long).Show();
            return;
        }

        if (selected is Pawn pawn)
        {
            //pawn.isFirstMove = false;
            pawn.EnPassantCapturable = move.EnPassantCapturable;
            if (space.GetBoardIndex().Item1 == (pawn.isWhite ? 'H' : 'A'))
                pawn.Promote();
        }

        SelectMoves(space);
        selected.Move(space);
        this.NextPlayer();
        return;
    }

    private void ClearSelectedMoves()
    {
        moves.Clear();
        foreach (var space in highlighted)
            space.space.Drawable.SetTint(space.isWhite ? UnselectedWhiteColor : UnselectedBlackColor);
    }

    private void SelectMoves()
    {
        ClearSelectedMoves();
        moves = this.selected.Moves(board, pieces);
        foreach (var move in moves)
        {
            var space = move.Space.BoardSpace(board);
            space.space.Drawable.SetTint(space.isWhite ? SelectedWhiteColor : SelectedBlackColor);
            highlighted.Add(space);
        }
        var selected = this.selected.BoardSpace(board);
        selected.space.Drawable.SetTint(selected.isWhite ? SelectedWhiteColor : SelectedBlackColor);
        highlighted.Add(selected);
    }

    private void SelectMoves(Piece piece) => SelectMoves(piece.BoardSpace(board));

    private void SelectMoves(Space space)
    {
        ClearSelectedMoves();
        var selected = this.selected.BoardSpace(board);
        space.space.Drawable.SetTint(space.isWhite ? SelectedWhiteColor : SelectedBlackColor);
        selected.space.Drawable.SetTint(selected.isWhite ? SelectedWhiteColor : SelectedBlackColor);
        highlighted.Add(selected);
        highlighted.Add(space);
    }

    private void NextPlayer()
    {

        selected = null;
        //Next player logic
    }

    private void OnNextPlayer()
    {
        //Update pieces and board
    }

    private void InitChessPieces()
    {
        //Init resources
        _ = new Space(base.Resources);

        int spaceId;
        Action callback = OnNextPlayer;
        spaceId = Resource.Id.gmb__A8;
        bRook1 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__bRook1), Resource.Id.gmp__bRook1,
        base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bRook", 1)] = bRook1;

        spaceId = Resource.Id.gmb__B8;
        bKnight1 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__bKnight1), Resource.Id.gmp__bKnight1,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bKnight", 1)] = bKnight1;

        spaceId = Resource.Id.gmb__C8;
        bBishop1 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__bBishop1), Resource.Id.gmp__bBishop1,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bBishop", 1)] = bBishop1;

        spaceId = Resource.Id.gmb__D8;
        bQueen = new Queen(base.FindViewById<ImageView>(Resource.Id.gmp__bQueen1), Resource.Id.gmp__bQueen1,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bQueen", 1)] = bQueen;

        spaceId = Resource.Id.gmb__E8;
        bKing = new King(base.FindViewById<ImageView>(Resource.Id.gmp__bKing1), Resource.Id.gmp__bKing1,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bKing", 1)] = bKing;

        spaceId = Resource.Id.gmb__F8;
        bBishop2 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__bBishop2), Resource.Id.gmp__bBishop2,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bBishop", 2)] = bBishop2;

        spaceId = Resource.Id.gmb__G8;
        bKnight2 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__bKnight2), Resource.Id.gmp__bKnight2,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bKnight", 2)] = bKnight2;

        spaceId = Resource.Id.gmb__H8;
        bRook2 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__bRook2), Resource.Id.gmp__bRook2,
            base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
        pieces[("bRook", 2)] = bRook2;

        int bPawnIndex = 0;
        for (int i = Resource.Id.gmp__bPawn1; i <= Resource.Id.gmp__bPawn8; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("bPawn"))
            {
                spaceId = Resource.Id.gmb__A7 + (8 * bPawnIndex);
                bPawns[bPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), false, spaceId, callback);
                pieces[("bPawn", bPawnIndex + 1)] = bPawns[bPawnIndex];
                bPawnIndex++;
                continue;
            }
        }

        spaceId = Resource.Id.gmb__A1;
        wRook1 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__wRook1), Resource.Id.gmp__wRook1,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wRook", 1)] = wRook1;

        spaceId = Resource.Id.gmb__B1;
        wKnight1 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__wKnight1), Resource.Id.gmp__wKnight1,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wKnight", 1)] = wKnight1;

        spaceId = Resource.Id.gmb__C1;
        wBishop1 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__wBishop1), Resource.Id.gmp__wBishop1,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wBishop", 1)] = wBishop1;

        spaceId = Resource.Id.gmb__D1;
        wQueen = new Queen(base.FindViewById<ImageView>(Resource.Id.gmp__wQueen1), Resource.Id.gmp__wQueen1,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wQueen", 1)] = wQueen;

        spaceId = Resource.Id.gmb__E1;
        wKing = new King(base.FindViewById<ImageView>(Resource.Id.gmp__wKing1), Resource.Id.gmp__wKing1,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wKing", 1)] = wKing;

        spaceId = Resource.Id.gmb__F1;
        wBishop2 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__wBishop2), Resource.Id.gmp__wBishop2,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wBishop", 2)] = wBishop2;

        spaceId = Resource.Id.gmb__G1;
        wKnight2 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__wKnight2), Resource.Id.gmp__wKnight2,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wKnight", 2)] = wKnight2;

        spaceId = Resource.Id.gmb__H1;
        wRook2 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__wRook2), Resource.Id.gmp__wRook2,
            base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
        pieces[("wRook", 2)] = wRook2;

        int wPawnIndex = 0;
        for (int i = Resource.Id.gmp__wPawn1; i <= Resource.Id.gmp__wPawn8; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("wPawn"))
            {
                spaceId = Resource.Id.gmb__A2 + (8 * wPawnIndex);
                wPawns[wPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), true, spaceId, callback);
                pieces[("wPawn", wPawnIndex + 1)] = wPawns[wPawnIndex];
                wPawnIndex++;
            }
        }
    }

    private void InitChessBoard()
    {
        bool isWhite = true;
        const string IsWhite = nameof(IsWhite);
        const string IsBlack = nameof(IsBlack);
        for (int i = Resource.Id.gmb__A1, j = 1; i <= Resource.Id.gmb__A8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };

            board[('A', j)] = new Space(space, isWhite, i);

        }

        for (int i = Resource.Id.gmb__B1, j = 1; i <= Resource.Id.gmb__B8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('B', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__C1, j = 1; i <= Resource.Id.gmb__C8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('C', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__D1, j = 1; i <= Resource.Id.gmb__D8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('D', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__E1, j = 1; i <= Resource.Id.gmb__E8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('E', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__F1, j = 1; i <= Resource.Id.gmb__F8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('F', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__G1, j = 1; i <= Resource.Id.gmb__G8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('G', j)] = new Space(space, isWhite, i);
        }

        for (int i = Resource.Id.gmb__H1, j = 1; i <= Resource.Id.gmb__H8; i++, j++)
        {
            var space = base.FindViewById<ImageView>(i);
            string tag = (space.Tag as Java.Lang.String).ToString();
            isWhite = tag switch
            {
                IsWhite => true,
                IsBlack => false,
                _ => throw new System.Exception($"{Resources.GetResourceEntryName(i)}: Missing a tag"),
            };
            board[('H', j)] = new Space(space, isWhite, i);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
