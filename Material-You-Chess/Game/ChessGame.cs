using System.Text;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;
using Newtonsoft.Json;

namespace Chess.Game;

public class ChessGame : IChessGame
{
    private int Turn = 1;
    private bool? clientPlayerIsWhite;
    private bool CurrentPlayerIsWhite = true;
    private readonly ConstraintLayout BoardLayout;

    public Dictionary<(string, int), IPiece> AllPieces { get; } = [];

    public Dictionary<(char, int), ISpace> Board { get; } = [];

    public IMove? LastMove { get; set; } = null;

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


    public IPlayer? White { get; set; }

    public IPlayer? Black { get; set; }
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

    private Action<Payload>? send;

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

    public ChessGame(ConstraintLayout boardLayout, bool? clientPlayerIsWhite, Action<Payload>? send)
    {
        this.BoardLayout = boardLayout;
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

        this.White = new White(this.Board, this.BoardLayout);
        this.Black = new Black(this.Board, this.BoardLayout);

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

        if (this.send != null)
        {
            var json = JsonConvert.SerializeObject(move, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var utf8 = UTF8Encoding.UTF8.GetBytes(json);
            this.send(Payload.FromBytes(utf8));
        }

        this.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
        this.OnMove(move);
        this.Selected!.Move(move.Destination);
        this.NextTurn();
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
        //Toast.MakeText()
    }

    public void OnMove(IMove move)
    {
        if (this.Player == null || this.Enemy == null)
            return;

        if (this.Selected is ISpecialBoardPiece piece)
        {
            piece.HasMoved = true;
        }

        if (move is DoubleMove pawn)
        {
            pawn.Pawn.EnPassantCapturable = true;
        }

        if (move is not ICapture capture)
        {
            return;
        }

        this.OnCapture(capture);
    }
}