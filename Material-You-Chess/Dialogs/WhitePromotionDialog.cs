using Android.Content;
using Chess.Game;
using Chess.Game.Moves;
using Google.Android.Material.Dialog;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace Chess.Dialogs;

public class WhitePromotionDialog : IPromotionDialog
{
    public AlertDialog Dialog { get; set; }
    public MaterialAlertDialogBuilder Builder { get; set; }
    public IPromote? Move { get; set; }
    private ChessGame? game;
    public WhitePromotionDialog(Context app)
    {
        this.Builder = new MaterialAlertDialogBuilder(app);
        this.Builder.SetTitle(nameof(Promote));
        this.Builder.SetView(Resource.Layout.white_promotion_dialog);
        this.Dialog = this.Builder.Create();
        this.Dialog.ShowEvent += this.OnShow;
    }

    public void Show(ChessGame game, IPromote Move)
    {
        this.game = game;
        this.Move = Move;
        this.Dialog.Show();
    }

    public void OnShow(object? sender, EventArgs args)
    {
        this.Dialog.FindViewById<ImageView>(Resource.Id.whitePromoteQueen)!.Click += (s, e) =>
        {
            this.Move = (this.Move is IPromoteAndCapture) switch
            {
                true => new PromoteQueenAndCapture((this.Move as IPromoteAndCapture)!.Pawn, (this.Move as IPromoteAndCapture)!.Piece),
                false => new PromoteQueen(this.Move!.Pawn, this.Move.Destination)
            };
            this.OnConfirm(s, e);
        };

        this.Dialog.FindViewById<ImageView>(Resource.Id.whitePromoteKnight)!.Click += (s, e) =>
        {
            this.Move = (this.Move is IPromoteAndCapture) switch
            {
                true => new PromoteKnightAndCapture((this.Move as IPromoteAndCapture)!.Pawn, (this.Move as IPromoteAndCapture)!.Piece),
                false => new PromoteKnight(this.Move!.Pawn, this.Move.Destination)
            };
            this.OnConfirm(s, e);
        };

        this.Dialog.FindViewById<ImageView>(Resource.Id.whitePromoteRook)!.Click += (s, e) =>
        {
            this.Move = (this.Move is IPromoteAndCapture) switch
            {
                true => new PromoteRookAndCapture((this.Move as IPromoteAndCapture)!.Pawn, (this.Move as IPromoteAndCapture)!.Piece),
                false => new PromoteRook(this.Move!.Pawn, this.Move.Destination)
            };
            this.OnConfirm(s, e);
        };

        this.Dialog.FindViewById<ImageView>(Resource.Id.whitePromoteBishop)!.Click += (s, e) =>
        {
            this.Move = (this.Move is IPromoteAndCapture) switch
            {
                true => new PromoteBishopAndCapture((this.Move as IPromoteAndCapture)!.Pawn, (this.Move as IPromoteAndCapture)!.Piece),
                false => new PromoteBishop(this.Move!.Pawn, this.Move.Destination)
            };
            this.OnConfirm(s, e);
        };
    }

    public void OnConfirm(object? sender, EventArgs args)
    {
        this.Dialog.Dismiss();
        this.game!.OnMove(this.Move!);
        this.game.Selected!.Move(this.Move!.Destination);
        this.game.NextTurn();
    }
}
