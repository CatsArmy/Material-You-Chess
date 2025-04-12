using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.App.Dialogs;
using Chess.Game;
using Chess.Game.Common;
using Google.Android.Material.ImageView;

namespace Chess.App;

public interface IChessActivity
{
    public static IChessActivity? Instance { get; set; }

    /// <summary> This Client</summary>
    UserClient Client { get; set; }

    /// <summary> The client that we connect to </summary>
    UserClient ConnectedClient { get; set; }
    ChessGame Game { get; set; }
    ChessBottomSheet? BottomSheet { get; set; }
    (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    TextView? WhitePlayerUsername { get; set; }
    TextView? BlackPlayerUsername { get; set; }
    ConstraintLayout? BoardLayout { get; set; }
    void Send(Payload payload);
    void Finish();
}
