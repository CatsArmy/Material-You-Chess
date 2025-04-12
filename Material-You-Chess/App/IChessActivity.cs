using Android.Content;
using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.App.Networked.Nearby;
using Chess.Game;
using Chess.Game.Common;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;

namespace Chess.App;

public interface IChessActivity
{
    public static IChessActivity? Instance { get; set; }
    public Context? Context { get; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }

    /// <summary> This Client</summary>
    public UserClient Client { get; set; }

    /// <summary> The client that we connect to </summary>
    public UserClient ConnectedClient { get; set; }
    public ChessGame Game { get; set; }
    ChessBottomSheet? BottomSheet { get; set; }

    public void Send(Payload payload);

    void Finish();

    T? FindViewById<T>(int id) where T : View;
}
