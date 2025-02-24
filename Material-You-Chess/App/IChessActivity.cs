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
    public ShapeableImageView? Player1ShapeableImageView { get; set; }
    public ShapeableImageView? Player2ShapeableImageView { get; set; }
    public TextView? Profile1Username { get; set; }
    public TextView? Profile2Username { get; set; }
    public ConstraintLayout? BoardLayout { get; set; }

    public string? Player1Name { get; }
    public string? Player2Name { get; }

    public void Send(Payload payload);
}
