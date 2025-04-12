using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App;
using Chess.App.Networked.Nearby;
using Chess.Game.Interfaces;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;

namespace Chess.Game.Common;

public class ChessBottomSheet(IChessActivity activity)
{
    public readonly CoordinatorLayout StandardBottomSheet = activity.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet)!;
    public readonly ConstraintLayout BottomSheetLayout = activity.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet)!;
    public readonly ConstraintLayout GameOverLayout = activity.FindViewById<ConstraintLayout>(Resource.Id.game_over)!;
    public readonly ConstraintLayout MatchmakingLayout = activity.FindViewById<ConstraintLayout>(Resource.Id.matchmaking)!;

    public readonly ImageView Indicator = activity.FindViewById<ImageView>(Resource.Id.winningPlayerIndicator)!;
    public readonly ShapeableImageView WinningPlayer = activity.FindViewById<ShapeableImageView>(Resource.Id.winningPlayer)!;
    public readonly TextView WinnerUsername = activity.FindViewById<TextView>(Resource.Id.winningPlayerUsername)!;
    public readonly TextView WinnerDescription = activity.FindViewById<TextView>(Resource.Id.winnerDescription)!;
    public readonly ExtendedFloatingActionButton Home = activity.FindViewById<ExtendedFloatingActionButton>(Resource.Id.home)!;

    public ChipGroup? MatchmakingPreferences = null;
    public readonly Chip White = activity.FindViewById<Chip>(Resource.Id.white_chip)!;
    public readonly Chip Black = activity.FindViewById<Chip>(Resource.Id.black_chip)!;
    public readonly TextView SearchingText = activity.FindViewById<TextView>(Resource.Id.SearchingText)!;
    public readonly CircularProgressIndicator SearchingIndicator = activity.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator)!;

    public readonly BottomSheetCallback Callback = new(activity);
    public readonly BottomSheetBehavior Behavior = BottomSheetBehavior.From(activity.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet)!);

    public static ChessBottomSheet OnCreate(IChessActivity activity)
    {
        var result = new ChessBottomSheet(activity);
        result.StandardBottomSheet!.Visibility = ViewStates.Invisible;
        result.Behavior.AddBottomSheetCallback(result.Callback);
        result.Behavior.State = BottomSheetBehavior.StateHidden;
        return result;
    }

    public void Show()
    {
        this.StandardBottomSheet!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Visible;
        this.GameOverLayout!.Visibility = ViewStates.Gone;
        this.Behavior!.State = BottomSheetBehavior.StateExpanded;

        if (this.MatchmakingPreferences is not null)
            return;

        this.MatchmakingPreferences = activity.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref)!;
        this.MatchmakingPreferences!.CheckedChange += this.OnPreferencesChange;
    }

    public void ShowGameOver(IPlayer player, string description)
    {
        this.StandardBottomSheet!.Visibility = ViewStates.Visible;
        this.GameOverLayout!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Gone;
        this.Behavior!.State = BottomSheetBehavior.StateExpanded;

        this.WinnerUsername!.Text = player.Name;
        this.WinnerDescription!.Text = description;
        switch (player)
        {
            case Player.Black:
                this.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                this.Indicator?.SetImageResource(Resource.Drawable.black_king);
                break;
            default:
                this.WinningPlayer?.SetImageDrawable(activity.WhitePlayerProfilePicture?.Drawable);
                break;
        }

        this.Home!.Click += this.FinishActivity;
    }

    private void OnPreferencesChange(object? sender, EventArgs args)
    {
        if (activity is not NetworkedChessActivity networked) return;

        if (!this.White!.Checked && !this.Black!.Checked)
        {
            networked.OnSelectNone();
        }

        if (this.White!.Checked)
        {
            networked.OnSelectWhite();
        }

        if (this.Black!.Checked)
        {
            networked.OnSelectBlack();
        }
    }

    private void FinishActivity(object? sender, EventArgs args)
    {
        IChessActivity.Instance = null;
        activity.Finish();
    }
}
