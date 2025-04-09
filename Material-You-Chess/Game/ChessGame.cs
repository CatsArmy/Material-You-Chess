using System.Linq;
using System.Text.Json;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using Chess.App;
using Chess.App.Networked;
using Chess.Game.Board;
using Chess.Game.Moves;
using Chess.Game.Player;
using static Java.Util.Jar.Attributes;

namespace Chess.Game;

// Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
#pragma warning disable CS9124 
public class ChessGame(IChessActivity activity)
{
    public ChessGame(NetworkedChessActivity activity) : this(activity as IChessActivity)
    {
        ChessGame.Instance = this;
        this.BindGame();
    }

    public ChessGame(ChessActivity activity) : this(activity as IChessActivity)
    {
        ChessGame.Instance = this;
        this.BindGame();
    }

    private readonly bool? ClientIsWhite = activity.Client switch
    {
        WhitePlayerClient => true,
        BlackPlayerClient => false,
        _ => null,
    };
    private bool CurrentPlayerIsWhite = true;

    public static ChessGame? Instance { get; set; }
    public IChessActivity Activity { get; } = activity;
    public Dictionary<(string Prefix, int Count), BoardPiece> AllPieces { get; } = [];
    public Dictionary<(char file, int rank), BoardSpace> Board { get; } = [];
    public White? WhitePlayer { get; set; }
    public Black? BlackPlayer { get; set; }

    /// <summary>The current player </summary>
    public IPlayer Player => this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer!,
        false => this.BlackPlayer!,
    };

    /// <summary>the enemy of <see cref="Player"/></summary>
    public IPlayer Enemy => !this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer!,
        false => this.BlackPlayer!,
    };

    /// <summary>
    /// The function <see cref="Validate(Java.Lang.String, out ValueTuple{string, int}, out ValueTuple{char, int})"/>s
    /// who the <paramref name="sender"/> is either a <see cref="BoardSpace"/>or a <see cref="BoardPiece"/>.
    /// based on the above the function will either select(<see cref="IPlayer.Selected"/>),
    /// move(<see cref="PlayMove(Move, bool)"/>) or ignore the click (do nothing)
    /// based on the state of the <see cref="CurrentPlayerIsWhite"/> and <see cref="ClientIsWhite"/>
    /// </summary> <param name="sender">the <see cref="ImageView"/> that was clicked</param>
    public void OnClick(object? sender, EventArgs args)
    {
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
            this.Player.Selected = null;
        else if (move is Promotion promotion && promotion.PromoteTo is null)
            this.PlayMove(move, false);
        else
            this.PlayMove(move, true);
    }

    /// <summary>Moves the piece with the given <paramref name="move"/> and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    /// <param name="isSender">if true will also send the move for the other player client to listen for when in an online game</param>
    public void PlayMove(Move move, bool isSender)
    {
        if (isSender)
            this.Activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: move, SourceJsonGenerationContext.Default.Move)));
        else
            this.PlayMove(this.LocalizeMove(move));
    }

    /// <summary>Moves the piece with the given <paramref name="move"/> and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    private void PlayMove(Move move)
    {
        this.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing); //Move, selected are null
        this.Player.Selected!.Move(move, this); // System.NullReferenceException: 'Object reference not set to an instance of an object.'
        if (move is Promotion promotion && promotion.PromoteTo is null) return; //prevent moving when no move is done
        this.Player.Selected = null;
        this.Player.LastMove = move;
        move?.Origin.Update();

        if (move is Castling castle) //the only edge case where the move contains multiple moves
        {
            castle.Rook?.Update();
            castle.Rook?.Move(castle.PlayRook);
        }

        this.Enemy.LastMove?.Origin.Update();
        this.Enemy.LastMove = null;

        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }

    private Move LocalizeMove(Move received)
    {
        var move = this.AllPieces[received.Origin.Index].Moves(this).FirstOrDefault((m)
            => received.Destination.Index == m.Destination.Index);
        if (move is Promotion promotion)
        {
            promotion.PromoteTo = (received as Promotion)!.PromoteTo;
        }

        return move!;
    }

    /// <summary> outputs the indexes of the space and or piece of the clicked <see cref="ImageView"/> </summary>
    /// <param name="Tag">the index of the space and or piece</param>
    /// <param name="pIndex">the index of the piece that might have been clicked</param>
    /// <param name="sIndex">the index of the space that was clicked</param>
    private void Validate(Java.Lang.String Tag, out (string, int) pIndex, out (char, int) sIndex)
    {
        string tag = Tag.ToString();
        sIndex = (tag[0], int.Parse($"{tag[^1]}"));
        pIndex = (tag[0..^1], int.Parse($"{tag[^1]}"));
        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len <= 2|   unknown |   unknown | uppercase &   len > 2 

        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length <= 2) || (char.IsUpper(sIndex.Item1) && pIndex.Item1.Length > 2))
            return; //invalid tag

        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if ((char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2))
        {
            if (!this.AllPieces.TryGetValue(pIndex, out BoardPiece? value))
                return;

            sIndex = value.Space.Index;
        }
    }

    /// <summary> Binds all the views to the class that manages them.
    /// for example: (<see cref="ImageView"/> id="gmb__A1") will be bound to a BoardSpace <br />
    /// creating a <see cref="IPlayer"/>(<see cref="White"/> or <see cref="Black"/>) 
    /// will bind the <see cref="ImageView"/>s of the pieces of that <see cref="IPlayer"/><br />
    /// For example: (<see cref="ImageView"/> id="gmp__wPawn1") will be bound to a <see cref="WhitePawn"/> <br />
    /// For example: (<see cref="ImageView"/> id="gmp__bPawn1") will be bound to a <see cref="BlackPawn"/> </summary>
    /// <remarks>Fills up(binds) both of the dictionaries: <see cref="AllPieces"/>, <see cref="Board"/></remarks>
    private void BindGame()
    {
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

        this.WhitePlayer = this.ClientIsWhite switch
        {
            false => new White(this, activity.ConnectedClient),
            _ => new White(this, activity.Client),
        };

        this.BlackPlayer = this.ClientIsWhite switch
        {
            false => new Black(this, activity.Client),
            _ => new Black(this, activity.ConnectedClient),
        };

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
    }

    /// <summary>
    /// Binds the <see cref="ImageView"/> with the <paramref name="id"/> to a <see cref="BoardSpace"/> 
    /// in the index of (<paramref name="file"/>,<paramref name="rank"/>)
    /// </summary>
    /// <param name="id">The id of the <see cref="ImageView"/> that will be bound</param>
    /// <param name="rank">The rank of the <see cref="BoardSpace"/>: (e.g if the space is "B2" then the rank will be '2')</param>
    /// <param name="file">The file of the <see cref="BoardSpace"/>: (e.g if the space is "B2" then the rank will be 'B')</param>
    /// <returns>The bounded <see cref="BoardSpace"/></returns>
    private BoardSpace BindSpace(int id, int rank, char file)
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

        space!.Tag = new Java.Lang.String($"{file}{rank}");
        space!.Clickable = true;
        if (!space.IsAttachedToWindow)
        {
            //(Activity as Android.App.Activity).WindowManager.AddView()
            space.ViewAttachedToWindow += (sender, args) => args.AttachedView.Click += this.OnClick;
        }
        else
            space!.Click += this.OnClick;

        return new BoardSpace(file, rank, isWhite, space!);
    }
}
#pragma warning restore CS9124
// Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
