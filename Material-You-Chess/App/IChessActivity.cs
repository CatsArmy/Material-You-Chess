using Android.Content;
using Android.Gms.Nearby.Connection;
using AndroidX.ConstraintLayout.Widget;
using Chess.Dialogs;
using Google.Android.Material.ImageView;

namespace Chess.App;

public interface IChessActivity
{
    public Context? Context { get; set; }
    public (WhitePromotionDialog White, BlackPromotionDialog Black) PromotionDialogs { get; set; }
    public ShapeableImageView? WhitePlayerProfilePicture { get; set; }
    public ShapeableImageView? BlackPlayerProfilePicture { get; set; }
    public TextView? WhitePlayerUsername { get; set; }
    public TextView? BlackPlayerUsername { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }

    public string? WhitePlayerName { get; }
    public string? BlackPlayerName { get; }

    public void Send(Payload payload);
}
