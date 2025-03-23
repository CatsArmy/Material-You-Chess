using Android.Content;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.App.Common;
using Chess.Dialogs;
using Google.Android.Material.ImageView;

namespace Chess.App;

public interface IChessActivity
{
    public Context? Context { get; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }

    /// <summary> This Client</summary>
    public UserClient? Client { get; set; }

    /// <summary> The client that we connect to </summary>
    public UserClient? ConnectedClient { get; set; }

    public void Send(Payload payload);
}
