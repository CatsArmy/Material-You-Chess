using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.ImageView;

namespace Chess;


[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
public class ChessActivity : AppCompatActivity
{
    public bool MaterialYouThemePreference;
    private string PlayerName = "Guest2";
    private AppPermissions permissions;
    private Android.Net.Uri uri;

    private Dictionary<(string, int), BoardSpace> board = new Dictionary<(string, int), BoardSpace>();
    private BoardPiece selected = null;

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

        foreach (var space in board.Values)
        {
            space.space.Click += (sender, e) => OnClick(sender, e, space);
        }
    }

    private void OnClick(object sender, System.EventArgs e, BoardSpace space)
    {
        if (selected == null)
        {
            if (space is BoardPiece piece)
                selected = piece;
            return;
        }

        if (selected.Move((BoardSpace)space, board))
            this.NextPlayer();
    }

    private void NextPlayer()
    {
        selected = null;
        //Next player logic
    }

    private void InitChessPieces()
    {
        int bPawnIndex = 0;
        bool bIsFirstBishop = true;
        bool bIsFirstKnight = true;
        bool bIsFirstRook = true;
        for (int i = Resource.Id.gmp__bBishop1; i <= Resource.Id.gmp__bRook2; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("bBishop"))
            {
                int spaceId;
                switch (bIsFirstBishop)
                {
                    case true:
                        bIsFirstBishop = true;
                        spaceId = Resource.Id.gmb__C8;
                        bBishop1 = new Bishop(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bBishop", 1)] = bBishop1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__F8;
                        bBishop2 = new Bishop(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bBishop", 2)] = bBishop2;
                        continue;
                };
            }
            if (a.Contains("bKing"))
            {
                int spaceId = Resource.Id.gmb__E8;
                bKing = new King(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), false, spaceId);
                board[("bKing", 1)] = bKing;
            }
            if (a.Contains("bKnight"))
            {
                int spaceId;
                switch (bIsFirstKnight)
                {
                    case true:
                        bIsFirstKnight = true;
                        spaceId = Resource.Id.gmb__B8;
                        bKnight1 = new Knight(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bKnight", 1)] = bKnight1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__G8;
                        bKnight2 = new Knight(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bKnight", 2)] = bKnight2;
                        continue;
                };
            }
            if (a.Contains("bPawn"))
            {
                int spaceId = Resource.Id.gmb__A7 + (8 * (bPawnIndex + 1));
                bPawns[bPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), false, spaceId);
                board[("bPawn", bPawnIndex + 1)] = bPawns[bPawnIndex];
                bPawnIndex++;
            }
            if (a.Contains("bQueen"))
            {
                int spaceId = Resource.Id.gmb__D8;
                bQueen = new Queen(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), false, spaceId);
                board[("bQueen", 1)] = bQueen;
            }
            if (a.Contains("bRook"))
            {
                int spaceId;
                switch (bIsFirstRook)
                {
                    case true:
                        bIsFirstRook = true;
                        spaceId = Resource.Id.gmb__A8;
                        bRook1 = new Rook(base.FindViewById<ImageView>(i), i,
                        base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bRook", 1)] = bRook1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__H8;
                        bRook2 = new Rook(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("bRook", 2)] = bRook2;
                        continue;
                };
            }
        }

        int wPawnIndex = 0;
        bool wIsFirstBishop = true;
        bool wIsFirstKnight = true;
        bool wIsFirstRook = true;
        for (int i = Resource.Id.gmp__wBishop1; i <= Resource.Id.gmp__wRook2; i++)
        {
            var a = Resources.GetResourceName(i);
            if (a.Contains("wBishop"))
            {
                int spaceId;
                switch (wIsFirstBishop)
                {
                    case true:
                        wIsFirstBishop = true;
                        spaceId = Resource.Id.gmb__C1;
                        wBishop1 = new Bishop(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), true, spaceId);
                        board[("wBishop", 1)] = wBishop1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__F1;
                        wBishop2 = new Bishop(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), true, spaceId);
                        board[("wBishop", 2)] = wBishop2;
                        continue;
                };
            }
            if (a.Contains("wKing"))
            {
                int spaceId = Resource.Id.gmb__E1;
                wKing = new King(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), true, spaceId);
                board[("wKing", 1)] = wKing;
            }
            if (a.Contains("wKnight"))
            {
                int spaceId;
                switch (wIsFirstKnight)
                {
                    case true:
                        wIsFirstKnight = true;
                        spaceId = Resource.Id.gmb__B1;
                        wKnight1 = new Knight(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), true, spaceId);
                        board[("wKnight", 1)] = wKnight1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__G1;
                        wKnight2 = new Knight(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), true, spaceId);
                        board[("wKnight", 2)] = wKnight2;
                        continue;
                };
            }
            if (a.Contains("wPawn"))
            {
                int spaceId = Resource.Id.gmb__A2 + (8 * (wPawnIndex + 1));
                wPawns[wPawnIndex] = new Pawn(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), true, spaceId);
                board[("wPawn", wPawnIndex + 1)] = wPawns[wPawnIndex];
                wPawnIndex++;
            }
            if (a.Contains("wQueen"))
            {
                int spaceId = Resource.Id.gmb__D1;
                wQueen = new Queen(base.FindViewById<ImageView>(i), i,
                    base.FindViewById<ImageView>(spaceId), true, spaceId);
                board[("wQueen", 1)] = wQueen;
            }
            if (a.Contains("wRook"))
            {
                int spaceId;
                switch (wIsFirstRook)
                {
                    case true:
                        wIsFirstRook = true;
                        spaceId = Resource.Id.gmb__A1;
                        wRook1 = new Rook(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), false, spaceId);
                        board[("wRook", 1)] = wRook1;
                        continue;

                    case false:
                        spaceId = Resource.Id.gmb__H1;
                        wRook2 = new Rook(base.FindViewById<ImageView>(i), i,
                            base.FindViewById<ImageView>(spaceId), true, spaceId);
                        board[("wRook", 2)] = wRook2;
                        continue;
                };
            }

        }
    }
    private void InitChessBoard()
    {
        bool isWhite = true;
        for (int i = Resource.Id.gmb__A1, j = 1; i <= Resource.Id.gmb__A8; i++, j++)
        {
            board[("A", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__B1, j = 1; i <= Resource.Id.gmb__B8; i++, j++)
        {
            board[("B", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__C1, j = 1; i <= Resource.Id.gmb__C8; i++, j++)
        {
            board[("C", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__D1, j = 1; i <= Resource.Id.gmb__D8; i++, j++)
        {
            board[("D", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__E1, j = 1; i <= Resource.Id.gmb__E8; i++, j++)
        {
            board[("E", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        isWhite = !isWhite;
        for (int i = Resource.Id.gmb__F1, j = 1; i <= Resource.Id.gmb__F8; i++, j++)
        {
            board[("F", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        for (int i = Resource.Id.gmb__G1, j = 1; i <= Resource.Id.gmb__G8; i++, j++)
        {
            board[("G", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
            isWhite = !isWhite;
        }
        for (int i = Resource.Id.gmb__H1, j = 1; i <= Resource.Id.gmb__H8; i++, j++)
        {
            board[("H", j)] = new BoardSpace(base.FindViewById<ImageView>(i), isWhite, i);
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
