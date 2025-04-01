using Android.Content;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common;
using Chess.App.Nearby;
using Chess.App.Networked;
using Chess.Dialogs;
using Chess.Game.Player;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;

namespace Chess.App;

public interface IChessActivity // Investigate bug cant play more than once per app session
{
    public Context? Context { get; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }

    public BottomSheetCallback? Callback { get; set; }
    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }
    public ConstraintLayout? MatchmakingLayout { get; set; }
    public ConstraintLayout? GameOverLayout { get; set; }
    public ExtendedFloatingActionButton? Home { get; set; }
    public ImageView? Indicator { get; set; }
    public ShapeableImageView? WinningPlayer { get; set; }
    public TextView? WinnerUsername { get; set; }
    public TextView? WinnerDescription { get; set; }

    /// <summary> This Client</summary>
    public UserClient Client { get; set; }

    /// <summary> The client that we connect to </summary>
    public UserClient ConnectedClient { get; set; }

    public virtual void Send(Payload payload) { return; }

    public void EndGame(IPlayer winner, IPlayer loser);
    public void Finish();
}
