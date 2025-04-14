using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App;
using Chess.Game.Player;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;

namespace Chess.Game.Common;

public class ChessBottomSheet(ChessActivity activity, CoordinatorLayout standardBottomSheet)
{
    public readonly ConstraintLayout BottomSheetLayout = standardBottomSheet.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet)!;
    public readonly ConstraintLayout GameOverLayout = standardBottomSheet.FindViewById<ConstraintLayout>(Resource.Id.game_over)!;
    public readonly ConstraintLayout MatchmakingLayout = standardBottomSheet.FindViewById<ConstraintLayout>(Resource.Id.matchmaking)!;

    public readonly ImageView Indicator = standardBottomSheet.FindViewById<ImageView>(Resource.Id.winningPlayerIndicator)!;
    public readonly ShapeableImageView WinningPlayer = standardBottomSheet.FindViewById<ShapeableImageView>(Resource.Id.winningPlayer)!;
    public readonly TextView WinnerUsername = standardBottomSheet.FindViewById<TextView>(Resource.Id.winningPlayerUsername)!;
    public readonly TextView WinnerDescription = standardBottomSheet.FindViewById<TextView>(Resource.Id.winnerDescription)!;
    public readonly ExtendedFloatingActionButton Home = standardBottomSheet.FindViewById<ExtendedFloatingActionButton>(Resource.Id.home)!;

    public ChipGroup? MatchmakingPreferences = null;
    public readonly Chip White = standardBottomSheet.FindViewById<Chip>(Resource.Id.white_chip)!;
    public readonly Chip Black = standardBottomSheet.FindViewById<Chip>(Resource.Id.black_chip)!;
    public readonly TextView SearchingText = standardBottomSheet.FindViewById<TextView>(Resource.Id.SearchingText)!;
    public readonly CircularProgressIndicator SearchingIndicator = standardBottomSheet.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator)!;

    public readonly BottomSheetCallback Callback = new(activity);
    public readonly BottomSheetBehavior Behavior = BottomSheetBehavior.From(standardBottomSheet.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet)!);

    public static ChessBottomSheet OnCreate(ChessActivity activity, CoordinatorLayout standardBottomSheet)
    {
        var result = new ChessBottomSheet(activity, standardBottomSheet);
        standardBottomSheet!.Visibility = ViewStates.Invisible;
        result.Behavior.AddBottomSheetCallback(result.Callback);
        result.Behavior.State = BottomSheetBehavior.StateHidden;
        return result;
    }

    public void ShowMatchmaking()
    {
        standardBottomSheet!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Visible;
        this.GameOverLayout!.Visibility = ViewStates.Gone;
        this.Behavior!.State = BottomSheetBehavior.StateExpanded;

        if (this.MatchmakingPreferences is not null) return;

        this.MatchmakingPreferences = standardBottomSheet.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref)!;
        this.MatchmakingPreferences!.CheckedChange += this.OnPreferencesChange;
    }

    /// <summary>Shows the Bottom Sheet and only shows the game over layout </summary>
    /// <param name="player">the player that ended the game</param>
    /// <param name="description">how the game ended</param>
    public void ShowGameOver(IPlayer player, string description)
    {
        standardBottomSheet!.Visibility = ViewStates.Visible;
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
        if (!activity.IsNetworked) return;

        if (!this.White!.Checked && !this.Black!.Checked) this.OnSelectNone(activity);
        if (this.White!.Checked) this.OnSelectWhite(activity);
        if (this.Black!.Checked) this.OnSelectBlack(activity);
    }

    public void OnSelectNone(ChessActivity networked)
    {
        this.SearchingIndicator?.Hide();
        this.SearchingText!.Text = "Please select a matchmaking preference";
        networked.IsAdvertising = false;
        networked.IsDiscovering = false;
    }

    public void OnSelectWhite(ChessActivity networked)
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
        networked.IsAdvertising = true;
    }

    public void OnSelectBlack(ChessActivity networked)
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
        networked.IsDiscovering = true;
    }

    private void FinishActivity(object? sender, EventArgs args)
    {
        activity.Finish();
    }
}
