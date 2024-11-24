using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
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

    public static Resources res;

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
        res = Resources;
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
        //string debug = string.Empty;
        //debug += $"{bRook1}\n{bKnight1}\n{bBishop1}\n";
        //debug += $"{bRook1.piece.Clickable}, {bKnight1.piece.Clickable}, {bBishop1.piece.Clickable}";
        //p1MainUsername.Text = debug;

        //TextView p2MainUsername = base.FindViewById<TextView>(Resource.Id.p2MainUsername);
        //string debug2 = string.Empty;
        //debug2 += $"{wRook1}\n{wKnight1}\n{wBishop1}\n";
        //debug2 += $"{wRook1.piece.Clickable}, {wKnight1.piece.Clickable}, {wBishop1.piece.Clickable}";
        //p2MainUsername.Text = debug2;
        selected = null;

        foreach (var space in board.Values)
            space.space.Click += (sender, e) => OnClickSpace(sender, e, space);

        foreach (var piece in pieces.Values)
        {
            piece.piece.Click += (sender, e) => OnClickPiece(sender, e, piece);
            //piece.space.Clickable = false;
            piece.piece.Clickable = true;
        }
    }

    private void OnClickPiece(object sender, System.EventArgs e, Piece piece)
    {
        TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        p1MainUsername.Text = $"{piece}";
        if (selected == null)
        {
            selected = piece;
            //selected.space.Clickable = false;
            //selected.piece.Clickable = false;
            //Log.Debug($"{nameof(OnClickPiece)}", $"{Resources.GetResourceName(selected.spaceId)} || {Resources.GetResourceName(selected.id)} ||  selected was null");
            return;
        }

        if (selected.isWhite == piece.isWhite)
        {
            //selected.space.Clickable = true;
            selected = piece;
            //selected.space.Clickable = false;
        }

        if (selected.id == piece.id)
            return;

        //Log.Debug($"{nameof(OnClickPiece)}", $"{Resources.GetResourceName(selected.spaceId)} || {Resources.GetResourceName(selected.id)} ||  selected was not null");
        if (selected.Move(piece, board, pieces))
            this.NextPlayer();
    }
    private void OnClickSpace(object sender, System.EventArgs e, Space space)
    {
        TextView p1MainUsername = base.FindViewById<TextView>(Resource.Id.p1MainUsername);
        p1MainUsername.Text = $"{space}";

        if (selected == null)
        {
            //Log.Debug($"{nameof(OnClickSpace)}", $"selected was null");
            return;
        }

        //Log.Debug($"{nameof(OnClickSpace)}", $"{Resources.GetResourceName(selected.spaceId)} || {Resources.GetResourceName(selected.id)} ||  selected was not null");
        if (selected.Move(space, board, pieces))
            this.NextPlayer();
    }

    private void NextPlayer()
    {
        selected = null;
        //Next player logic
    }

    private void InitChessPieces()
    {
        //Init resources
        Piece.SetResources(base.Resources);

        int spaceId;

        spaceId = Resource.Id.gmb__A8;
        bRook1 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__bRook1), Resource.Id.gmp__bRook1,
        base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bRook", 1)] = bRook1;

        spaceId = Resource.Id.gmb__B8;
        bKnight1 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__bKnight1), Resource.Id.gmp__bKnight1,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bKnight", 1)] = bKnight1;

        spaceId = Resource.Id.gmb__C8;
        bBishop1 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__bBishop1), Resource.Id.gmp__bBishop1,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bBishop", 1)] = bBishop1;

        spaceId = Resource.Id.gmb__D8;
        bQueen = new Queen(base.FindViewById<ImageView>(Resource.Id.gmp__bQueen), Resource.Id.gmp__bQueen,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bQueen", 1)] = bQueen;

        spaceId = Resource.Id.gmb__E8;
        bKing = new King(base.FindViewById<ImageView>(Resource.Id.gmp__bKing), Resource.Id.gmp__bKing,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bKing", 1)] = bKing;

        spaceId = Resource.Id.gmb__F8;
        bBishop2 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__bBishop2), Resource.Id.gmp__bBishop2,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bBishop", 2)] = bBishop2;

        spaceId = Resource.Id.gmb__G8;
        bKnight2 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__bKnight2), Resource.Id.gmp__bKnight2,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bKnight", 2)] = bKnight2;

        spaceId = Resource.Id.gmb__H8;
        bRook2 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__bRook2), Resource.Id.gmp__bRook2,
            base.FindViewById<ImageView>(spaceId), false, spaceId);
        pieces[("bRook", 2)] = bRook2;

        int bPawnIndex = 0;
        for (int i = Resource.Id.gmp__bPawn1; i <= Resource.Id.gmp__bPawn8; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("bPawn"))
            {
                spaceId = Resource.Id.gmb__A7 + (8 * bPawnIndex);
                bPawns[bPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), false, spaceId);
                pieces[("bPawn", bPawnIndex + 1)] = bPawns[bPawnIndex];
                bPawnIndex++;
                continue;
            }
        }

        spaceId = Resource.Id.gmb__A1;
        wRook1 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__wRook1), Resource.Id.gmp__wRook1,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wRook", 1)] = wRook1;

        spaceId = Resource.Id.gmb__B1;
        wKnight1 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__wKnight1), Resource.Id.gmp__wKnight1,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wKnight", 1)] = wKnight1;

        spaceId = Resource.Id.gmb__C1;
        wBishop1 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__wBishop1), Resource.Id.gmp__wBishop1,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wBishop", 1)] = wBishop1;

        spaceId = Resource.Id.gmb__D1;
        wQueen = new Queen(base.FindViewById<ImageView>(Resource.Id.gmp__wQueen), Resource.Id.gmp__wQueen,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wQueen", 1)] = wQueen;

        spaceId = Resource.Id.gmb__E1;
        wKing = new King(base.FindViewById<ImageView>(Resource.Id.gmp__wKing), Resource.Id.gmp__wKing,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wKing", 1)] = wKing;

        spaceId = Resource.Id.gmb__F1;
        wBishop2 = new Bishop(base.FindViewById<ImageView>(Resource.Id.gmp__wBishop2), Resource.Id.gmp__wBishop2,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wBishop", 2)] = wBishop2;

        spaceId = Resource.Id.gmb__G1;
        wKnight2 = new Knight(base.FindViewById<ImageView>(Resource.Id.gmp__wKnight2), Resource.Id.gmp__wKnight2,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wKnight", 2)] = wKnight2;

        spaceId = Resource.Id.gmb__H1;
        wRook2 = new Rook(base.FindViewById<ImageView>(Resource.Id.gmp__wRook2), Resource.Id.gmp__wRook2,
            base.FindViewById<ImageView>(spaceId), true, spaceId);
        pieces[("wRook", 2)] = wRook2;

        int wPawnIndex = 0;
        for (int i = Resource.Id.gmp__wPawn1; i <= Resource.Id.gmp__wPawn8; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("wPawn"))
            {
                spaceId = Resource.Id.gmb__A2 + (8 * wPawnIndex);
                wPawns[wPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), true, spaceId);
                pieces[("wPawn", wPawnIndex + 1)] = wPawns[wPawnIndex];
                wPawnIndex++;
            }
        }
    }
    private void InitChessBoard()
    {
        bool isWhite = true;
        for (int i = Resource.Id.gmb__A1, j = 1; i <= Resource.Id.gmb__A8; i++, j++)
        {
            board[('A', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__B1, j = 1; i <= Resource.Id.gmb__B8; i++, j++)
        {
            board[('B', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__C1, j = 1; i <= Resource.Id.gmb__C8; i++, j++)
        {
            board[('C', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__D1, j = 1; i <= Resource.Id.gmb__D8; i++, j++)
        {
            board[('D', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__E1, j = 1; i <= Resource.Id.gmb__E8; i++, j++)
        {
            board[('E', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__F1, j = 1; i <= Resource.Id.gmb__F8; i++, j++)
        {
            board[('F', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        for (int i = Resource.Id.gmb__G1, j = 1; i <= Resource.Id.gmb__G8; i++, j++)
        {
            board[('G', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        for (int i = Resource.Id.gmb__H1, j = 1; i <= Resource.Id.gmb__H8; i++, j++)
        {
            board[('H', j)] = new Space(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        // Handle permission requests results
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
