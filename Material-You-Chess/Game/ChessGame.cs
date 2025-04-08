using System.Text.Json;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using Chess.App;
using Chess.App.Common;
using Chess.App.Networked;
using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;

namespace Chess.Game;

public class ChessGame
{
    public static ChessGame? Instance { get; set; }

    public IChessActivity Activity;
    public White WhitePlayer { get; set; }
    public Black BlackPlayer { get; set; }

    private bool IsOver = false;
    private bool CurrentPlayerIsWhite = true;
    private readonly bool? ClientIsWhite;

    public Dictionary<(string Prefix, int Count), BoardPiece> AllPieces { get; }
    public Dictionary<(char file, int rank), BoardSpace> Board { get; }

    public IPlayer Player => this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer,
        false => this.BlackPlayer,
    };

    public IPlayer Enemy => !this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer,
        false => this.BlackPlayer
    };

    public BoardSpace BindSpace(int id, int rank, char file)
    {
        const string IsWhite = "IsWhite";
        const string IsBlack = "IsBlack";
        var space = this.Activity.BoardLayout!.FindViewById<ImageView>(id);
        string? tag = (space?.Tag as Java.Lang.String)?.ToString();
        bool isWhite = tag switch
        {
            IsWhite => true,
            IsBlack => false,
            _ => throw new Exception($"{this.Activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
        };

        space!.Click += this.OnClick;
        space!.Tag = new Java.Lang.String($"{file}{rank}");
        space!.Clickable = true;

        return new BoardSpace(file, rank, isWhite, space!);
    }

    public ChessGame(IChessActivity activity)
    {
        ChessGame.Instance = this;
        this.Board = [];
        this.AllPieces = [];
        this.Activity = activity;
        this.ClientIsWhite = activity.Client switch
        {
            WhitePlayerClient => true,
            BlackPlayerClient => false,
            _ => null,
        };

        char file = 'A';
        for (int id = Resource.Id.gmb__A1, rank = 1; id <= Resource.Id.gmb__A8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //B

        for (int id = Resource.Id.gmb__B1, rank = 1; id <= Resource.Id.gmb__B8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++;  //C

        for (int id = Resource.Id.gmb__C1, rank = 1; id <= Resource.Id.gmb__C8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //D

        for (int id = Resource.Id.gmb__D1, rank = 1; id <= Resource.Id.gmb__D8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //E

        for (int id = Resource.Id.gmb__E1, rank = 1; id <= Resource.Id.gmb__E8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //F

        for (int id = Resource.Id.gmb__F1, rank = 1; id <= Resource.Id.gmb__F8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //G

        for (int id = Resource.Id.gmb__G1, rank = 1; id <= Resource.Id.gmb__G8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);
        file++; //H

        for (int id = Resource.Id.gmb__H1, rank = 1; id <= Resource.Id.gmb__H8; id++, rank++)
            this.Board[(file, rank)] = this.BindSpace(id, rank, file);

        switch (this.ClientIsWhite)
        {
            case false:
                this.WhitePlayer = new White(this, activity.ConnectedClient);
                this.BlackPlayer = new Black(this, activity.Client);
                break;

            default:
                this.WhitePlayer = new White(this, activity.Client);
                this.BlackPlayer = new Black(this, activity.ConnectedClient);
                break;
        }

        var players = (Dictionary<(string Prefix, int Count), BoardPiece>[])[this.WhitePlayer.Pieces, this.BlackPlayer.Pieces];

        foreach (var player in players)
        {
            foreach (var kvp in player)
            {
                var index = kvp.Key;
                var piece = kvp.Value;
                if (this.AllPieces.ContainsKey(index))
                    continue;

                piece.PieceView!.Tag = new Java.Lang.String($"{piece.Prefix}{piece.Count}");
                piece.PieceView!.Click += this.OnClick;
                piece.PieceView!.Clickable = true;
                this.AllPieces[index] = piece;
            }
        }

        Logger.Debug("Created game");
    }

    public void PlayMove(Move move, bool isSender)
    {
        if (!isSender)
        {
            this.PlayMove(move);
            return;
        }

        this.Activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: move, SourceJsonGenerationContext.Default.Move)));
        this.PlayMove(move);
    }

    public void Cleanup()
    {
        this.IsOver = true;
    }

    private void PlayMove(Move move)
    {
        this.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
        this.Player.Selected!.Move(move, this);
        this.Player.Selected = null;
        this.Player.LastMove = move;

        if (move?.Origin is SpecialPiece piece) // Update Our Player Pieces state if it is needed
            piece.Update();

        if (move is Castling castle) //the only edge case where the move contains multiple moves
        {
            castle.Rook?.Update();
            castle.Rook?.Move(castle.PlayRook);
        }

        this.Enemy.LastMove?.Origin.Update();
        this.Enemy.LastMove = null;

        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }

    public void OnClick(object? sender, EventArgs args)
    {
        if (this.IsOver) return;

        if (sender is not ImageView imageView)
            return;

        if (imageView?.Tag is not Java.Lang.String javaString)
            return;

        if (this.ClientIsWhite != null)
            if (this.ClientIsWhite != this.CurrentPlayerIsWhite)
                return;

        this.Validate(javaString, out var pIndex, out var sIndex);
        if (this.Player!.Pieces.TryGetValue(pIndex, out BoardPiece? Piece))
        {
            if (this.Player.Selected == null)
            {
                this.Player.Selected = Piece;
                return;
            }

            if (this.Player.Selected.IsWhite == Piece.IsWhite)
            {
                if (this.Player.Selected.Id != Piece.Id)
                    this.Player.Selected = Piece;
                else
                    this.Player.Selected = null;
                return;
            }
        }

        if (!this.Board.TryGetValue(sIndex, out var space))
            return;

        var move = this.Player.Moves?.FirstOrDefault(move => move.Destination.Index == space.Index);
        if (move is null)
            return;

        this.PlayMove(move, true);
    }

    public bool Validate(Java.Lang.String Tag, out (string, int) pIndex, out (char, int) sIndex)
    {
        string tag = Tag.ToString();
        sIndex = (tag[0], int.Parse($"{tag[^1]}"));
        pIndex = (tag[0..^1], int.Parse($"{tag[^1]}"));
        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len <= 2|   unknown |   unknown | uppercase &   len > 2 

        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length <= 2) || (char.IsUpper(sIndex.Item1) && pIndex.Item1.Length > 2))
            return false; //invalid tag

        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2))
        {
            if (!this.AllPieces.TryGetValue(pIndex, out BoardPiece? value))
                return true;

            sIndex = value.Space.Index;
        }

        return true;
    }
}
