using Android.Content;
using Chess.Game;
using Chess.Game.Board;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public class BlackPromotionDialog : IPromotionDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public Promotion? Move { get; set; }
    public Pawn? Caller { get; set; }

    private ChessGame? game;

    public List<int> Id => [Resource.Id.blackPromoteQueen, Resource.Id.blackPromoteKnight, Resource.Id.blackPromoteRook,
        Resource.Id.blackPromoteBishop];

    public BlackPromotionDialog(Context app)
    {
        this.Builder = new MaterialAlertDialogBuilder(app);
        this.Builder.SetTitle(nameof(Promotion));
        this.Builder.SetView(Resource.Layout.black_promotion_dialog);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += this.OnShow;
    }

    public void Show(ChessGame game, Pawn pawn, Promotion move)
    {
        this.game = game;
        this.Caller = pawn;
        this.Move = move;
        this.Dialog.Show();
    }

    public void OnShow(object? sender, EventArgs args)
    {
        foreach (var id in this.Id)
            this.Dialog.FindViewById<ImageView>(id)!.Click += this.OnConfirm;
    }

    public void OnConfirm(object? sender, EventArgs args)
    {
        this.Dialog.Dismiss();
        var type = (sender as ImageView)!.Id switch
        {
            Resource.Id.blackPromoteQueen => typeof(Queen),
            Resource.Id.blackPromoteKnight => typeof(Knight),
            Resource.Id.blackPromoteRook => typeof(Rook),
            Resource.Id.blackPromoteBishop => typeof(Bishop),
            _ => null
        };

        if (type is null)
            return;

        this.Move!.PromoteTo = new(type);
        this.Caller?.Move(this.Move, this.game!);
    }
}
