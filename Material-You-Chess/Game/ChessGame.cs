using System.Text.Json;
using Android.Animation;
using Android.Gms.Nearby.Connection;
using Android.Views;
using Google.Android.Material.Badge;
using Google.Android.Material.Floatingtoolbar;
using Google.Android.Material.ImageView;
using Material.You.Chess.Game.Board;
using Material.You.Chess.Game.Common;
using Material.You.Chess.Game.Moves;
using Material.You.Chess.Game.Player;

namespace Material.You.Chess.Game;

public class ChessGame
{
    public readonly Dictionary<(string Prefix, int Count), BoardPiece> AllPieces;
    public readonly Dictionary<(char file, int rank), BoardSpace> Board;
    private readonly ChessActivity Activity;
    public readonly White WhitePlayer;
    public readonly Black BlackPlayer;
    public readonly Player.Player White;
    public readonly Player.Player Black;

    /// <summary> The current player </summary>
    public IPlayer Player => this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer,
        false => this.BlackPlayer,
    };

    /// <summary> The enemy of the current player </summary>
    public IPlayer Enemy => !this.CurrentPlayerIsWhite switch
    {
        true => this.WhitePlayer,
        false => this.BlackPlayer,
    };

    private bool CurrentPlayerIsWhite = true;
    public static BadgeDrawable? CaptureAlertTopRightBadge;
    public static BadgeDrawable? CaptureAlertTopLeftBadge;
    public static BadgeDrawable? CaptureAlertBottomRightBadge;
    public static BadgeDrawable? CaptureAlertBottomLeftBadge;

    /// <summary> Initializes(binds) the board and players(and pieces) and adds on click listeners </summary>
    public ChessGame(ChessActivity activity, Player.Player white, Player.Player black)
    {
        this.Activity = activity;
        CaptureAlertTopRightBadge = BadgeDrawable.Create(activity);
        CaptureAlertTopRightBadge.BadgeGravity = BadgeDrawable.TopEnd;
        CaptureAlertTopLeftBadge = BadgeDrawable.Create(activity);
        CaptureAlertTopLeftBadge.BadgeGravity = BadgeDrawable.TopStart;
        CaptureAlertBottomRightBadge = BadgeDrawable.Create(activity);
        CaptureAlertBottomRightBadge.BadgeGravity = BadgeDrawable.BottomEnd;
        CaptureAlertBottomLeftBadge = BadgeDrawable.Create(activity);
        CaptureAlertBottomLeftBadge.BadgeGravity = BadgeDrawable.BottomStart;
        #region Binds the board spaces
        this.Board = [];
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

        #region Binds the players and all pieces 
        this.AllPieces = [];
        this.White = white;
        this.WhitePlayer = new(this)
        {
            QuickPromotionAction = activity.FindViewById<FloatingToolbarLayout>(Resource.Id.ftbWhite_promotion)!
        };

        activity.FindViewById(Resource.Id.promote_white_queen)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_white_knight)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_white_rook)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_white_bishop)!.Click += Promotion;

        this.Black = black;
        this.BlackPlayer = new(this)
        {
            QuickPromotionAction = activity.FindViewById<FloatingToolbarLayout>(Resource.Id.ftbBlack_promotion)!
        };

        activity.FindViewById(Resource.Id.promote_black_queen)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_black_knight)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_black_rook)!.Click += Promotion;
        activity.FindViewById(Resource.Id.promote_black_bishop)!.Click += Promotion;

        for (int i = Resource.Id.gmp__bBishop1; i <= Resource.Id.gmp__wRook2; i++)
        {
            var view = this.Activity.BoardLayout!.FindViewById(i);
            view!.Click += this.OnClick;
        }
        #endregion
    }

    public void EndGame(IPlayer winner, IPlayer loser)
    {
        foreach (var view in this.AllPieces.Values) view.PieceView!.Clickable = false;
        foreach (var view in this.Board.Values) view.SpaceView!.Clickable = false;

        winner.IsCheckmated = false;
        loser.IsCheckmated = true;
        this.Activity.BottomSheet?.ShowGameOver(winner, $"{winner.Name} Wins, {loser.Name} loses");
    }

    /// <summary>
    /// The function Validates who the sender is either a BoardSpace or a BoardPiece
    /// based on the above the function will either select(Player.Selected),
    /// move(PlayMove(Move, bool)) or ignore the click (do nothing)
    /// </summary> 
    /// <param name="sender">the View that was clicked</param>
    public void OnClick(object? sender, EventArgs args)
    {
        if (this.Activity.IsNetworked && this.Activity?.Player?.IsWhite != this.CurrentPlayerIsWhite)
            return; // its not our clients turn

        if ((sender as View)?.Tag is not Java.Lang.String tag)
            return; // missing a tag

        if (!this.TryGetIndexes(tag, out var pieceIndex, out var spaceIndex))
            return; // Failed to extract the indexes of the space and piece

        // check if the piece index is a piece index or a spaceIndex disguised by a string(instead of a char).
        if (this.Player!.Pieces.TryGetValue(pieceIndex, out var piece))
        {
            if (this.Player.Selected == null)
            {
                this.Player.Selected = piece;
                return;
            }

            if (this.Player.Selected.IsWhite == piece.IsWhite)
            {
                if (this.Player.Selected.Id != piece.Id)
                    this.Player.Selected = piece;
                else
                    this.Player.Selected = null;
                return;
            }
        }

        if (!this.Board.TryGetValue(spaceIndex, out var space)) return;

        var move = this.Player.Moves?.FirstOrDefault(move => move.Destination.Index == space.Index);
        if (move is null)
            this.Player.Selected = null;
        else if (move is Promotion promotion && promotion.PromoteTo is null) //prevent sending so that our user can first decide
            this.Player!.Promotion = promotion;
        else
            this.PlayMove(move, isSender: true);
    }

    /// <summary> Moves the piece with the given move and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    /// <param name="isSender">if true will also send the move for the other player client to listen for when in an online game</param>
    public void PlayMove(Move move, bool isSender)
    {
        if (isSender)
        {
            this.Activity.Send(Payload.FromBytes(JsonSerializer.SerializeToUtf8Bytes(value: move, SourceJsonGenerationContext.Default.Move)));
            return;
        }

        this.PlayMove(this.LocalizeMove(move));
    }

    private void Promotion(object? sender, EventArgs e)
    {
        var type = (sender as View)?.Id switch
        {
            Resource.Id.promote_white_queen => typeof(WhiteQueen),
            Resource.Id.promote_black_queen => typeof(BlackQueen),
            Resource.Id.promote_white_knight => typeof(WhiteKnight),
            Resource.Id.promote_black_knight => typeof(BlackKnight),
            Resource.Id.promote_white_rook => typeof(WhiteRook),
            Resource.Id.promote_black_rook => typeof(BlackRook),
            Resource.Id.promote_white_bishop => typeof(WhiteBishop),
            Resource.Id.promote_black_bishop => typeof(BlackBishop),
            _ => null
        };

        if (type is null) return;

        this.Player.Promotion!.PromoteTo = new(type);
        this.PlayMove(this.Player.Promotion!, true);
    }

    /// <summary>Moves the piece with the given move parameter and updates the state of the game </summary>
    /// <param name="move">the move that the piece will move to</param>
    private void PlayMove(Move move)
    {
        if (this.Activity.IsNetworked) // make sure that no matter which player plays a move it can play it
            this.Player.Selected = move.Origin;
        this.Activity.BoardLayout?.LayoutTransition?.EnableTransitionType(LayoutTransitionType.Changing);
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
        if (move is Promotion promotion) // pass data that cannot be generated without the players choice
            promotion.PromoteTo = (received as Promotion)!.PromoteTo;

        return move!;
    }

    /// <summary> outputs the indexes of the space and or piece of the clicked View </summary>
    /// <param name="javaTag">the index of the space and or piece</param>
    /// <param name="piece">the index of the piece that might have been clicked</param>
    /// <param name="space">the index of the space that was clicked</param>
    private bool TryGetIndexes(Java.Lang.String javaTag,
        out (string prefix, int count) piece,
        out (char file, int rank) space)
    {
        string tag = javaTag.ToString();
        space = (tag[0], int.Parse($"{tag[^1]}"));
        piece = (tag[0..^1], int.Parse($"{tag[^1]}"));

        //Example tag of a space: "A1", starts with uppercase and has a length of 2
        //Example tag of a piece: "bPawn1" starts with lowercase and has a length that is bigger than 2

        if ((char.IsLower(space.file) && piece.prefix.Length <= 2) || (char.IsUpper(space.file) && piece.prefix.Length > 2))
            return false; // unrecognized tag format

        if (!char.IsLower(space.file) || piece.prefix.Length <= 2)
            return true; // recognized tag format as a space

        // recognized tag format as a piece,
        // updating the space index to prevent losing data when clicking a piece view.
        if (this.AllPieces.TryGetValue(piece, out var Piece))
            space = Piece.Space.Index;

        return true; // recognized at least one index using the tag format.
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
        var space = this.Activity.BoardLayout!.FindViewById<ShapeableImageView>(id);
        string? tag = (space?.Tag as Java.Lang.String)?.ToString();
        bool isWhite = tag switch
        {
            IsWhite => true,
            IsBlack => false,
            _ => throw new Exception($"{this.Activity.BoardLayout!.Resources?.GetResourceEntryName(id)}: Missing color tag"),
        };

        space!.Tag = new Java.Lang.String($"{file}{rank}");
        space!.Clickable = true;
        space!.Click += this.OnClick;

        return new BoardSpace(file, rank, isWhite, space!);
    }
}
