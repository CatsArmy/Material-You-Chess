using Android.Content;
using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

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
        this.Builder = new MaterialAlertDialogBuilder(app);
        this.Builder.SetTitle(nameof(Promotion));
        this.Builder.SetView(Resource.Layout.white_promotion_dialog);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += this.OnShow;
    }

    public void Show(ChessGame game, Pawn pawn, Promotion move)
    {
        this.Game = game;
        this.Caller = pawn;
        this.Move = move;
        this.Dialog.Show();
    }

    public void OnShow(object? sender, EventArgs args)
    {
        foreach (var id in this.IDs) this.Dialog.FindViewById<ImageView>(id)!.Click += this.OnConfirm;
    }

    public void OnConfirm(object? sender, EventArgs args)
    {
        this.Dialog.Dismiss();
        foreach (var id in this.IDs) this.Dialog.FindViewById<ImageView>(id)!.Click -= this.OnConfirm;
        var type = (sender as ImageView)!.Id switch
        {
            Resource.Id.whitePromoteQueen => typeof(WhiteQueen),
            Resource.Id.whitePromoteKnight => typeof(WhiteKnight),
            Resource.Id.whitePromoteRook => typeof(WhiteRook),
            Resource.Id.whitePromoteBishop => typeof(WhiteBishop),
            _ => null
        };

        if (type is null) return;

        this.Move!.PromoteTo = new(type);
        this.Game?.PlayMove(this.Move, true);
    }
}
