using Android.Content;
using Chess.Game.Board;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Game.Dialogs;

public class WhitePromotionDialog : IPromotionDialog
{
    private ChessGame? Game;

    public AlertDialog Dialog { get; set; }
    public AlertDialog.Builder Builder { get; set; }
    public Promotion? Move { get; set; }
    public Pawn? Caller { get; set; }
    public List<int> IDs => [Resource.Id.whitePromoteQueen, Resource.Id.whitePromoteKnight,
                                     Resource.Id.whitePromoteRook, Resource.Id.whitePromoteBishop];

    public WhitePromotionDialog(Context app)
    {
        Builder = new MaterialAlertDialogBuilder(app);
        Builder.SetTitle(nameof(Promotion));
        Builder.SetView(Resource.Layout.white_promotion_dialog);
        Dialog = Builder.Create();
        Dialog.ShowEvent += OnShow;
    }

    public void Show(ChessGame game, Pawn pawn, Promotion move)
    {
        Game = game;
        Caller = pawn;
        Move = move;
        Dialog.Show();
    }

    public void OnShow(object? sender, EventArgs args)
    {
        foreach (var id in IDs) Dialog.FindViewById<ImageView>(id)!.Click += OnConfirm;
    }

    public void OnConfirm(object? sender, EventArgs args)
    {
        Dialog.Dismiss();
        foreach (var id in IDs) Dialog.FindViewById<ImageView>(id)!.Click -= OnConfirm;
        var type = (sender as ImageView)!.Id switch
        {
            Resource.Id.whitePromoteQueen => typeof(WhiteQueen),
            Resource.Id.whitePromoteKnight => typeof(WhiteKnight),
            Resource.Id.whitePromoteRook => typeof(WhiteRook),
            Resource.Id.whitePromoteBishop => typeof(WhiteBishop),
            _ => null
        };

        if (type is null) return;

        Move!.PromoteTo = new(type);
        Game?.PlayMove(Move, true);
    }
}
