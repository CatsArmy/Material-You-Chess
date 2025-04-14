using System.Text.Json;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using Android.Views;
using Chess.App;
using Chess.Game.Board;
using Chess.Game.Common;
using Chess.Game.Moves;
using Chess.Game.Player;
using Google.Android.Material.ImageView;

namespace Chess.Game;

public class ChessGame(ChessActivity activity, bool isNetworked)
{
    public readonly bool ClientIsWhite = activity.Client.IsWhite ?? true;
    private bool CurrentPlayerIsWhite = true;

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

    /// <summary>the enemy of Player </summary>
    public IPlayer Enemy => !this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer!,
        false => this.BlackPlayer!,
    };

    public ChessGame(ChessActivity activity) : this(activity, activity.IsNetworked) => this.BindGame();

    public void EndGame(IPlayer winner, IPlayer loser)
    {
        foreach (var view in this.AllPieces.Values) view.PieceView!.Clickable = false;
        foreach (var view in this.Board.Values) view.SpaceView!.Clickable = false;

        this.Player!.Outcome = GameOutcome.Win;
        this.Enemy!.Outcome = GameOutcome.Lose;
        activity.BottomSheet?.ShowGameOver(winner, $"{winner.Name} Wins, {loser.Name} loses");
    }

    /// <summary>
    /// The function Validates who the sender is either a BoardSpace or a BoardPiece
    /// based on the above the function will either select(Player.Selected),
    /// move(PlayMove(Move, bool)) or ignore the click (do nothing)
    /// </summary> 
    /// <param name="sender">the View that was clicked</param>
    public void OnClick(object? sender, EventArgs args)
    {
        if (sender is not View view)
            return;

        if (view?.Tag is not Java.Lang.String javaString)
            return;

        if (isNetworked && this.ClientIsWhite != this.CurrentPlayerIsWhite)
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

    /// <summary> Moves the piece with the given move and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    /// <param name="isSender">if true will also send the move for the other player client to listen for when in an online game</param>
    public void PlayMove(Move move, bool isSender)
    {
        if (isSender)
            activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: move, SourceJsonGenerationContext.Default.Move)));
        else
            this.PlayMove(this.LocalizeMove(move));
    }

    /// <summary>Moves the piece with the given <paramref name="move"/> and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    private void PlayMove(Move move)
    {
        activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
        this.Player.Selected!.Move(move, this);
        if (move is Promotion promotion && promotion.PromoteTo is null) return; //prevent moving when no move is done
        this.Player.Selected = null;
        this.Player.LastMove = move;
        (move.Origin as SpecialPiece)?.Update();

        if (move is Castling castle) //the only edge case where the move contains multiple moves
        {
            castle.Rook?.Update();
            castle.Rook?.Move(castle.PlayRook);
        }

       (this.Enemy.LastMove?.Origin as SpecialPiece)?.Update();
        this.Enemy.LastMove = null;

        this.CurrentPlayerIsWhite = !this.CurrentPlayerIsWhite;
    }

    /// <remarks> ensures that when receiving a move it will reference classes that are in the lists on this client </remarks>
    /// <summary> Localizes the received move into a local move that contains references to pieces/spaces 
    /// that are in the AllPieces and or the Board(AllSpaces) </summary>
    /// <param name="received">a Move that was received from another client</param>
    private Move LocalizeMove(Move received)
    {
        var move = this.AllPieces[received.Origin.Index].Moves(this).FirstOrDefault((m) => received.Destination.Index == m.Destination.Index);
        if (move is Promotion promotion)
            promotion.PromoteTo = (received as Promotion)!.PromoteTo;

        return move!;
    }

    /// <summary> outputs the indexes of the space and or piece of the clicked View </summary>
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
            return;

        //  A1      |   bPawn1  |   case    |   case    |   bPawn1  |   A1
        //----------+-----------+-----------+-----------+-----------+-----------
        //lowercase &   len > 2 |   piece   |   space   | uppercase &   len = 0 
        if (char.IsLower(sIndex.Item1) && pIndex.Item1.Length > 2)
        {
            if (!this.AllPieces.TryGetValue(pIndex, out BoardPiece? value))
                return;

            sIndex = value.Space.Index;
        }
    }

    /// <remarks> Fills up(binds) both of the dictionaries: AllPieces, Board </remarks>
    /// <summary> Binds all the views to the class that manages them.
    /// for example: (ShapeableImageView id="gmb__A1") will be bound to a BoardSpace
    /// creating a IPlayer(White or Black) will bind the Buttons of the pieces of that Player
    /// For example: (Button id="gmp__wPawn1") will be bound to a WhitePawn
    /// For example: (Button id="gmp__bPawn1") will be bound to a BlackPawn </summary>
    private void BindGame()
    {
        #region Binds the board spaces
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
        #endregion

        #region Bind players
        this.WhitePlayer = this.ClientIsWhite switch
        {
            false => new White(this, activity.PromotionDialogs.White, activity.ConnectedClient),
            _ => new White(this, activity.PromotionDialogs.White, activity.Client),
        };

        this.BlackPlayer = this.ClientIsWhite switch
        {
            false => new Black(this, activity.PromotionDialogs.Black, activity.Client),
            _ => new Black(this, activity.PromotionDialogs.Black, activity.ConnectedClient),
        };
        #endregion
        // Set onclick listeners
        for (int i = Resource.Id.gmp__bBishop1; i <= Resource.Id.gmp__wRook2; i++)
        {
            var view = activity.BoardLayout!.FindViewById(i)!;
            view.Click += this.OnClick;
        }
    }

    /// <summary> Binds the View with the id to a BoardSpace in the index of (file, rank) </summary>
    /// <param name="id">The id of the View that will be bound</param>
    /// <param name="rank">The rank of the BoardSpace: (e.g if the space is "B2" then the rank will be '2')</param>
    /// <param name="file">The file of the BoardSpace: (e.g if the space is "B2" then the rank will be 'B')</param>
    /// <returns> The bounded BoardSpace </returns>
    private BoardSpace BindSpace(int id, int rank, char file)
    {
        const string IsWhite = "IsWhite";
        const string IsBlack = "IsBlack";
        var space = activity.BoardLayout!.FindViewById<ShapeableImageView>(id);
        string? tag = (space?.Tag as Java.Lang.String)?.ToString();
        bool isWhite = tag switch
        {
            IsWhite => true,
            IsBlack => false,
            _ => throw new Exception($"{activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
        };

        space!.Tag = new Java.Lang.String($"{file}{rank}");
        space!.Clickable = true;
        space!.Click += this.OnClick;

        return new BoardSpace(file, rank, isWhite, space!);
    }
}
