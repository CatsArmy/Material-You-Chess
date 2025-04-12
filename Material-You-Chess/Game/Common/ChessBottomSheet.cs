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
    public readonly BottomSheetCallback Callback = new(activity);

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
    public BottomSheetBehavior Behavior
    {
        get; private set
        {
            field = value;
            this.StandardBottomSheet!.Visibility = ViewStates.Invisible;
            value.State = BottomSheetBehavior.StateHidden;
            value!.AddBottomSheetCallback(this.Callback);
        }
    } = BottomSheetBehavior.From(activity.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet)!);

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

    public void ShowGameOver(IPlayer winner, IPlayer loser)
    {
        this.StandardBottomSheet!.Visibility = ViewStates.Visible;
        this.GameOverLayout!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Gone;
        this.Behavior!.State = BottomSheetBehavior.StateExpanded;

        this.WinnerUsername!.Text = winner.Name;
        this.WinnerDescription!.Text = $"{winner.Name} Wins, {loser.Name} loses";
        switch (winner)
        {
            case Player.Black:
                this.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                this.Indicator?.SetImageResource(Resource.Drawable.king_black);
                break;
            default:
                this.WinningPlayer?.SetImageDrawable(activity.WhitePlayerProfilePicture?.Drawable);
                break;
        }

        this.Home!.Click += this.FinishActivity;
    }

    public void ShowConnectionError()
    {
        this.GameOverLayout!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Gone;
        this.Behavior!.State = BottomSheetBehavior.StateExpanded;

        this.WinnerUsername!.Text = activity.ConnectedClient.Username;
        this.WinnerDescription!.Text = $"{activity.ConnectedClient.Username} disconnected, {activity.Client.Username} wins by technicality";

        switch (activity.Game.ClientIsWhite) // Inverse because our client did not disconnect
        {
            case true: //our client is white and the connect client is black
                this.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                this.Indicator?.SetImageResource(Resource.Drawable.king_black);
                break;
            default: //our client is black and the connect client is white
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

    public enum VisibilityState
    {
        Dragging = BottomSheetBehavior.StateDragging,
        Settling = BottomSheetBehavior.StateSettling,
        Expanded = BottomSheetBehavior.StateExpanded,
        HalfExpanded = BottomSheetBehavior.StateHalfExpanded,
        Collapsed = BottomSheetBehavior.StateCollapsed,
        Hidden = BottomSheetBehavior.StateHidden,
    }
}
