using Android.Animation;
using Android.Content.PM;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;
using Chess.Util;
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
    private string BoardState = string.Empty;

    private Dictionary<(string, int), IPiece> pieces = new();
    private Dictionary<(char, int), ISpace> board = new();
    private IPiece? selected
    {
        get; set
        {
            field = value;
            if (value is not null)
            {
                this.SelectMoves();
            }
            if (this.skipClearingSelected)
            {
                this.skipClearingSelected = false;
                return;
            }
            if (value is null)
            {
                this.ClearSelectedMoves();
            }
        }
    } = null;
    private bool skipClearingSelected = false;
    private List<ISpace> highlighted = new();
    private List<IMove>? moves = new();

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

        this.p1MainUsername!.Text = (FirebaseAuth.Instance?.CurrentUser == null) switch
        {
            true => "Guest",
            false => FirebaseAuth.Instance?.CurrentUser?.DisplayName,
        };
        Instance = this;

        this.InitChessBoard();
        this.InitChessPieces();
        //TODO implement En Passant difficulty Medium--
        //TODO Add Casstling difficulty Medium
        //TODO Add Promotion difficulty Easy+ / Medium-
        //TODO More?
        foreach (KeyValuePair<(char, int), ISpace> keyValuePair in this.board)
        {
            keyValuePair.Value.Space!.Click += OnClick;
            keyValuePair.Value.Space!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Space!.Clickable = true;
        }

        foreach (KeyValuePair<(string, int), IPiece> keyValuePair in this.pieces)
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
        var sIndex = SpaceTagToIndex(tag);
        var pIndex = PieceTagToIndex(tag);
        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len <= 2|   unknown |   unknown | uppercase &   len > 2 

        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length <= 2) || (char.IsUpper(sIndex.Item1) && pIndex.Item1.Length > 2))
            return;

        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2))
        {
            sIndex = this.pieces[pIndex].Space.Index;
            var piece = this.pieces[pIndex];
            if (this.selected == null)
            {
                this.selected = piece;
                return;
            }

            if (this.selected.IsWhite == piece.IsWhite)
            {
                if (this.selected.Id != piece.Id)
                {
                    this.selected = piece;
                }
                return;
            }
        }
        else if (this.selected == null)
            return;


        ISpace space = board[sIndex];
        if (this.moves?.FirstOrDefault(move => move.Destination.Index == space.Index) is not IMove move)
        {
            this.selected = null;
            return;
        }

        space!.SelectSpace();
        this.selected!.Space.SelectSpace();

        this.highlighted?.Add(space);
        this.highlighted?.Add(this.selected!.Space);
        if (this.selected is Pawn pawn)
        {
            if (move is Pawn.DoubleMove doubleMove)
            {
                doubleMove.Pawn.EnPassantCapturable = true;
            }
            else if (move is Pawn.EnPassant enPassant)
            {
                enPassant.Pawn.Space.SelectSpace();
                this.highlighted?.Add(enPassant.Pawn.Space);
                this.selected.Capture(enPassant.Pawn, pieces);
            }
            pawn.HasMoved = true;
        }
        else if (move is ICapture capture)
            this.selected.Capture(capture.Piece, pieces);

        base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard)?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);

        this.selected.Move(move.Destination);
        this.skipClearingSelected = true;
        this.selected = null;
    }
    public static (char, int) SpaceTagToIndex(string Tag) => (Tag[0], int.Parse($"{Tag[^1]}"));
    public static (string, int) PieceTagToIndex(string Tag) => (Tag[0..^1], int.Parse($"{Tag[^1]}"));
    /*
    private void OnClickPiece(object? sender, EventArgs args, IPiece piece)
    {
        this.p1MainUsername!.Text = $"{piece}";
        this.p2MainUsername!.Text = $"{nameof(piece)}.{nameof(piece.IsWhite)}:{piece.IsWhite}";

        if (this.selected == null)
        {
            this.selected = piece;
            return;
        }

        if (this.selected.IsWhite == piece.IsWhite)
        {
            if (this.selected.Id != piece.Id)
            {
                this.selected = piece;
            }
            return;
        }

        var move = this.moves!.FirstOrDefault(move => move.Destination.Index == piece.Space.Index);
        if (move == null)
        {
            this.selected = null;
            return;
        }

        piece!.Space!.SelectSpace();
        this.selected!.Space.SelectSpace();

        this.highlighted?.Add(piece.Space);
        this.highlighted?.Add(this.selected!.Space);
        if (move is IEnPassant enPassant)
        {
            enPassant.Pawn.Space.SelectSpace();
            this.highlighted?.Add(enPassant.Pawn.Space);
            this.selected.Capture(enPassant.Pawn, pieces);
        }
        else if (move is ICapture capture)
            this.selected.Capture(capture.Piece, pieces);
        else
            base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard)?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
        this.selected.Move(move.Destination!);
        this.selected = null;
    }

    private void OnClickSpace(object? sender, System.EventArgs e, ISpace space)
    {
        this.p1MainUsername!.Text = $"{space}";
        this.p2MainUsername!.Text = $"{nameof(space.IsWhite)}:{space.IsWhite}";

        if (this.selected == null)
            return;

        if (this.moves?.FirstOrDefault(move => move.Destination.Index == space.Index) is not IMove move)
        {
            this.selected = null;
            return;
        }

        var layout = base.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard);
        layout?.LayoutTransition?.EnableTransitionType(Android.Animation.LayoutTransitionType.Changing);
        //var (spaceId, spaceView) = this.selected!.FakeMove(space.Id, space.Space);
        //var king = this.selected.IsWhite ? this.wKing : this.bKing;
        //if (king!.IsInCheck(this.board, this.pieces))
        //{
        //    this.selected.FakeMove(spaceId, spaceView);
        //    Toast.MakeText(this, "Cant play that move would result in a check", ToastLength.Long)?.Show();
        //    return;
        //}

        if (this.selected is Pawn pawn)
        {
            pawn.HasMoved = false;
            if (space.Rank == (pawn.IsWhite ? 8 : 1))
                pawn.Promote();
        }

        ClearSelectedMoves();
        space.SelectSpace();
        this.selected!.Space.SelectSpace();

        this.highlighted?.Add(space);
        this.highlighted?.Add(this.selected!.Space);

        this.selected.Move(space);
        this.NextPlayer();
        return;
    }
    */
    private void ClearSelectedMoves()
    {
        this.moves?.Clear();
        foreach (var space in this.highlighted!)
            space.UnselectSpace();
    }

    private void SelectMoves()
    {
        this.ClearSelectedMoves();
        this.moves = this.selected?.Moves(this.board, this.pieces);
        if (this.moves is null || this.moves?.Count <= 0)
            return;

        foreach (var move in this.moves!)
        {
            move.Destination.SelectSpace();
            this.highlighted?.Add(move.Destination);
            if (move?.Origin?.Space?.Drawable?.Level == ISpace.Unselect)
            {
                move.Origin.SelectSpace();
                this.highlighted?.Add(move.Origin);
            }
        }
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
        this.bRook1 = new Rook(Resource.Id.gmp__bRook1, ("bRook", 1), false, this.board[(file, blackRank)]);
        this.pieces[("bRook", 1)] = this.bRook1;

        this.wRook1 = new Rook(Resource.Id.gmp__wRook1, ("wRook", 1), true, this.board[(file, whiteRank)]);
        this.pieces[("wRook", 1)] = this.wRook1;
        file++;//B

        this.bKnight1 = new Knight(Resource.Id.gmp__bKnight1, ("bKnight", 1), false, this.board[(file, blackRank)]);
        this.pieces[("bKnight", 1)] = this.bKnight1;

        this.wKnight1 = new Knight(Resource.Id.gmp__wKnight1, ("wKnight", 1), true, this.board[(file, whiteRank)]);
        this.pieces[("wKnight", 1)] = this.wKnight1;
        file++;//C

        this.bBishop1 = new Bishop(Resource.Id.gmp__bBishop1, ("bBishop", 1), false, this.board[(file, blackRank)]);
        this.pieces[("bBishop", 1)] = this.bBishop1;

        this.wBishop1 = new Bishop(Resource.Id.gmp__wBishop1, ("wBishop", 1), true, this.board[(file, whiteRank)]);
        this.pieces[("wBishop", 1)] = this.wBishop1;
        file++;//D

        this.bQueen = new Queen(Resource.Id.gmp__bQueen1, ("bQueen", 1), false, this.board[(file, blackRank)]);
        this.pieces[("bQueen", 1)] = this.bQueen;

        this.wQueen = new Queen(Resource.Id.gmp__wQueen1, ("wQueen", 1), true, this.board[(file, whiteRank)]);
        this.pieces[("wQueen", 1)] = this.wQueen;
        file++;//E

        this.bKing = new King(Resource.Id.gmp__bKing1, ("bKing", 1), false, this.board[(file, blackRank)]);
        this.pieces[("bKing", 1)] = this.bKing;

        this.wKing = new King(Resource.Id.gmp__wKing1, ("wKing", 1), true, this.board[(file, whiteRank)]);
        this.pieces[("wKing", 1)] = this.wKing;
        file++;//F

        this.bBishop2 = new Bishop(Resource.Id.gmp__bBishop2, ("bBishop", 2), false, this.board[(file, blackRank)]);
        this.pieces[("bBishop", 2)] = this.bBishop2;

        this.wBishop2 = new Bishop(Resource.Id.gmp__wBishop2, ("wBishop", 2), true, this.board[(file, whiteRank)]);
        this.pieces[("wBishop", 2)] = this.wBishop2;
        file++;//G

        this.bKnight2 = new Knight(Resource.Id.gmp__bKnight2, ("bKnight", 2), false, this.board[(file, blackRank)]);
        this.pieces[("bKnight", 2)] = this.bKnight2;

        this.wKnight2 = new Knight(Resource.Id.gmp__wKnight2, ("wKnight", 2), true, this.board[(file, whiteRank)]);
        this.pieces[("wKnight", 2)] = this.wKnight2;
        file++;//H

        this.bRook2 = new Rook(Resource.Id.gmp__bRook2, ("bRook", 2), false, this.board[(file, blackRank)]);
        this.pieces[("bRook", 2)] = this.bRook2;

        this.wRook2 = new Rook(Resource.Id.gmp__wRook2, ("wRook", 2), true, this.board[(file, whiteRank)]);
        this.pieces[("wRook", 2)] = this.wRook2;

        file = 'A';
        blackRank--;
        whiteRank++;

        for (int i = 0; i < 8; i++)
        {
            this.bPawns[i] = new Pawn(Resource.Id.gmp__bPawn1 + i, ("bPawn", i + 1), false, this.board[(file, blackRank)]);
            this.pieces[("bPawn", i + 1)] = bPawns[i];

            this.wPawns[i] = new Pawn(Resource.Id.gmp__wPawn1 + i, ("wPawn", i + 1), true, this.board[(file, whiteRank)]);
            this.pieces[("wPawn", i + 1)] = this.wPawns[i];
            file++;
        }
    }

    private void InitChessBoard()
    {
        const string isWhite = "IsWhite";
        const string isBlack = "IsBlack";
        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{this.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
        {
            var space = base.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
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
