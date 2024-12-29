using Android.Animation;
using AndroidX.ConstraintLayout.Widget;
using Chess.ChessBoard;
using Chess.Util;

namespace Chess.Game;


//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip
public interface IChessGame
{
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> AllPieces { get; }
    /*[IgnoreDataMember]*/
    public Dictionary<(char, int), ISpace> Board { get; }
    /*[IgnoreDataMember]*/
    public List<IMove>? Moves { get; set; }
    /*[DataMember]*/
    public IPlayer? Player1 { get; set; }
    /*[DataMember]*/
    public IPlayer? Player2 { get; set; }
    /*[DataMember]*/
    public IMove? LastMove { get; set; }
    /*[DataMember]*/
    public IPiece? Selected { get; set; }
}

public class ChessGame : IChessGame
{
    private int Turn = 1;
    private bool CurrentPlayerIsWhite = true;
    private readonly Activity app;
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

    public ChessGame(Activity activity)
    {
        this.app = activity;
        const string isWhite = "IsWhite";
        const string isBlack = "IsBlack";
        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
        {
            var space = activity.FindViewById<ImageView>(id);
            string? tag = (space?.Tag as Java.Lang.String)?.ToString();
            this.Board[(file, rank)] = new BoardSpace(space, file, rank, tag switch
            {
                isWhite => true,
                isBlack => false,
                _ => throw new Exception($"{activity.Resources?.GetResourceEntryName(id)}: Missing color tag"),
            }, id);
        }

        foreach (KeyValuePair<(char, int), ISpace> keyValuePair in this.Board)
        {
            keyValuePair.Value.Space!.Click += OnClick;
            keyValuePair.Value.Space!.Tag = new Java.Lang.String($"{keyValuePair.Key.Item1}{keyValuePair.Key.Item2}");
            keyValuePair.Value.Space!.Clickable = true;
        }

        this.Player1 = new White(this.Board, this.app);
        this.Player2 = new Black(this.Board, this.app);

        this.AllPieces.Merge(this.Player1.Pieces, this.Player2.Pieces);
        //foreach (var Piece in this.Player1.Pieces)
        //    this.AllPieces[Piece.Key] = Piece.Value;

        //foreach (var Piece in this.Player2.Pieces)
        //    this.AllPieces[Piece.Key] = Piece.Value;

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

        this.app.FindViewById<ConstraintLayout>(Resource.Id.ChessBoard)?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);

        this.Selected!.Move(move.Destination);
        this.Selected = null;
        if (!this.CurrentPlayerIsWhite)
            this.Turn += 1;
        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }
}


/*[DataContract]*/
public enum GameOutcome
{
    /*[DataMember]*/
    Draw,
    /*[DataMember]*/
    Resign,
    /*[DataMember]*/
    Win,
    /*[DataMember]*/
    Lose,
    /*[DataMember]*/
    Ongoing,
}

public interface IPlayer
{
    /*[DataMember]*/
    public GameOutcome Outcome { get; set; }
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> Pieces { get; set; }
    /*[DataMember]*/
    public Pawn[] Pawns { get; set; }
    /*[DataMember]*/
    public Rook? Rook1 { get; set; }
    /*[DataMember]*/
    public Knight? Knight1 { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop1 { get; set; }
    /*[DataMember]*/
    public King? King { get; set; }
    /*[DataMember]*/
    public Queen? Queen { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop2 { get; set; }
    /*[DataMember]*/
    public Knight? Knight2 { get; set; }
    /*[DataMember]*/
    public Rook? Rook2 { get; set; }
}

public class Black : IPlayer
{
    /*[DataMember]*/
    public GameOutcome Outcome { get; set; } = GameOutcome.Ongoing;
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> Pieces { get; set; } = [];
    /*[DataMember]*/
    public Pawn[] Pawns { get; set; } = new Pawn[8];
    /*[DataMember]*/
    public Rook? Rook1 { get; set; }
    /*[DataMember]*/
    public Knight? Knight1 { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop1 { get; set; }
    /*[DataMember]*/
    public King? King { get; set; }
    /*[DataMember]*/
    public Queen? Queen { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop2 { get; set; }
    /*[DataMember]*/
    public Knight? Knight2 { get; set; }
    /*[DataMember]*/
    public Rook? Rook2 { get; set; }

    public Black(Dictionary<(char, int), ISpace> Board, Activity app)
    {
        char file = 'A';
        const int rank = 8;
        this.Rook1 = new Rook(Resource.Id.gmp__bRook1, ("bRook", 1), false, Board[(file, rank)], app);
        this.Pieces[("bRook", 1)] = this.Rook1;
        file++;//B

        this.Knight1 = new Knight(Resource.Id.gmp__bKnight1, ("bKnight", 1), false, Board[(file, rank)], app);
        this.Pieces[("bKnight", 1)] = this.Knight1;
        file++;//C

        this.Bishop1 = new Bishop(Resource.Id.gmp__bBishop1, ("bBishop", 1), false, Board[(file, rank)], app);
        this.Pieces[("bBishop", 1)] = this.Bishop1;
        file++;//D

        this.Queen = new Queen(Resource.Id.gmp__bQueen1, ("bQueen", 1), false, Board[(file, rank)], app);
        this.Pieces[("bQueen", 1)] = this.Queen;
        file++;//E

        this.King = new King(Resource.Id.gmp__bKing1, ("bKing", 1), false, Board[(file, rank)], app);
        this.Pieces[("bKing", 1)] = this.King;
        file++;//F

        this.Bishop2 = new Bishop(Resource.Id.gmp__bBishop2, ("bBishop", 2), false, Board[(file, rank)], app);
        this.Pieces[("bBishop", 2)] = this.Bishop2;
        file++;//G

        this.Knight2 = new Knight(Resource.Id.gmp__bKnight2, ("bKnight", 2), false, Board[(file, rank)], app);
        this.Pieces[("bKnight", 2)] = this.Knight2;
        file++;//H

        this.Rook2 = new Rook(Resource.Id.gmp__bRook2, ("bRook", 2), false, Board[(file, rank)], app);
        this.Pieces[("bRook", 2)] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++, file++)
        {
            this.Pawns[i] = new Pawn(Resource.Id.gmp__bPawn1 + i, ("bPawn", i + 1), false, Board[(file, rank - 1)], app);
            this.Pieces[("bPawn", i + 1)] = this.Pawns[i];
        }
    }
}
public class White : IPlayer
{
    /*[DataMember]*/
    public GameOutcome Outcome { get; set; } = GameOutcome.Ongoing;
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> Pieces { get; set; } = [];
    /*[DataMember]*/
    public Pawn[] Pawns { get; set; } = new Pawn[8];
    /*[DataMember]*/
    public Rook? Rook1 { get; set; }
    /*[DataMember]*/
    public Knight? Knight1 { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop1 { get; set; }
    /*[DataMember]*/
    public King? King { get; set; }
    /*[DataMember]*/
    public Queen? Queen { get; set; }
    /*[DataMember]*/
    public Bishop? Bishop2 { get; set; }
    /*[DataMember]*/
    public Knight? Knight2 { get; set; }
    /*[DataMember]*/
    public Rook? Rook2 { get; set; }
    public White(Dictionary<(char, int), ISpace> Board, Activity app)
    {
        char file = 'A';
        const int rank = 1;
        this.Rook1 = new Rook(Resource.Id.gmp__wRook1, ("wRook", 1), true, Board[(file, rank)], app);
        this.Pieces[("wRook", 1)] = this.Rook1;
        file++;//B

        this.Knight1 = new Knight(Resource.Id.gmp__wKnight1, ("wKnight", 1), true, Board[(file, rank)], app);
        this.Pieces[("wKnight", 1)] = this.Knight1;
        file++;//C

        this.Bishop1 = new Bishop(Resource.Id.gmp__wBishop1, ("wBishop", 1), true, Board[(file, rank)], app);
        this.Pieces[("wBishop", 1)] = this.Bishop1;
        file++;//D

        this.Queen = new Queen(Resource.Id.gmp__wQueen1, ("wQueen", 1), true, Board[(file, rank)], app);
        this.Pieces[("wQueen", 1)] = this.Queen;
        file++;//E

        this.King = new King(Resource.Id.gmp__wKing1, ("wKing", 1), true, Board[(file, rank)], app);
        this.Pieces[("wKing", 1)] = this.King;
        file++;//F

        this.Bishop2 = new Bishop(Resource.Id.gmp__wBishop2, ("wBishop", 2), true, Board[(file, rank)], app);
        this.Pieces[("wBishop", 2)] = this.Bishop2;
        file++;//G

        this.Knight2 = new Knight(Resource.Id.gmp__wKnight2, ("wKnight", 2), true, Board[(file, rank)], app);
        this.Pieces[("wKnight", 2)] = this.Knight2;
        file++;//H

        this.Rook2 = new Rook(Resource.Id.gmp__wRook2, ("wRook", 2), true, Board[(file, rank)], app);
        this.Pieces[("wRook", 2)] = this.Rook2;

        file = 'A';
        for (int i = 0; i < 8; i++)
        {
            this.Pawns[i] = new Pawn(Resource.Id.gmp__wPawn1 + i, ("wPawn", i + 1), true, Board[(file, rank + 1)], app);
            this.Pieces[("wPawn", i + 1)] = this.Pawns[i];
            file++;
        }
    }

}

//public interface INetworkedPlayer : IPlayer
//{
//    public bool HasColorPreference { get; set; }
//}
//public class ChessGameManager
//{
//    public ChessGameManager(IChessGame game)
//    {
//        this.Game = game;
//    }

//    public IChessGame Game;
//}

//public class LocalGame : ILocalGame
//{

//}

//public class OnlineGame : IOnlineGame
//{

//}