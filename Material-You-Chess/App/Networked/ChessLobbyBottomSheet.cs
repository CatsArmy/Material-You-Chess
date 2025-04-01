using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.Activity;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common.Extensions;
using Chess.App.Common.Permissions;
using Chess.App.Nearby;
using Chess.Game;
using Chess.Game.Player;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.ImageView;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.Snackbar;

namespace Chess.App.Networked;

public partial class NetworkedChessActivity : ConnectionsActivity
{
    public ChessBottomSheet? ChessBottomSheet { get; set; }
    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }
    public ConstraintLayout? GameOverLayout { get; set; }
    public ConstraintLayout? MatchmakingLayout { get; set; }


    public TextView? SearchingText { get; set; }
    public CircularProgressIndicator? SearchingIndicator { get; set; }
    public ChipGroup? MatchmakingPreferences { get; set; }
    public Chip? White { get; set; }
    public Chip? Black { get; set; }

    public State State
    {
        get; set
        {
            this.OnStateChanged(field, value);
            field = value;
        }
    } = State.Idle;

    private NearbyConnections? PermissionManager;

    public virtual void OnSelectNone()
    {
        this.SearchingIndicator?.Hide();
        this.SearchingText!.Text = "Please select a matchmaking preference";
        this.State = State.Idle;
    }

    public virtual void OnSelectWhite()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
        this.State = State.Advertising;
    }

    public virtual void OnSelectBlack()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
        this.State = State.Discovering;
    }

    /// <param name="isGranted"> <paramref name="isGranted"/> are all of the requested permissions granted </param>
    public virtual void HandlePermissionResult(bool isGranted)
    {
        if (!isGranted)
        {
            this.SetResult(Result.Canceled);
            this.Finish();
            return;
        }

        this.StandardBottomSheet!.Visibility = ViewStates.Visible;
    }

    public void CreateBottomSheet()
    {
        this.PermissionManager = this.RegisterNearbyPermissionsManager(this.HandlePermissionResult);
        this.PermissionManager.RequestAccess();

        this.SearchingIndicator = base.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator);
        this.SearchingText = base.FindViewById<TextView>(Resource.Id.SearchingText);
        this.White = base.FindViewById<Chip>(Resource.Id.white_chip);
        this.Black = base.FindViewById<Chip>(Resource.Id.black_chip);
        this.MatchmakingPreferences = base.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref);
        this.MatchmakingPreferences!.CheckedChange += (_, _) =>
        {
            if (!this.White!.Checked && !this.Black!.Checked)
            {
                this.OnSelectNone();
            }

            if (this.White!.Checked)
            {
                this.OnSelectWhite();
            }

            if (this.Black!.Checked)
            {
                this.OnSelectBlack();
            }
        };
    }

    public void Hide()
    {
        this.MatchmakingLayout!.Visibility = ViewStates.Gone;
        this.GameOverLayout!.Visibility = ViewStates.Gone;
        this.BottomSheet!.RemoveBottomSheetCallback(this.Callback!);
    }

    public void Show()
    {
        this.GameOverLayout!.Visibility = ViewStates.Gone;
        this.StandardBottomSheet!.Visibility = ViewStates.Visible;
        this.MatchmakingLayout!.Visibility = ViewStates.Visible;
        this.BottomSheet!.AddBottomSheetCallback(this.Callback!);
        this.BottomSheet!.State = BottomSheetBehavior.StateHalfExpanded;
    }

    public void OnStateChanged(State currentState, State requestedState)
    {
        if (currentState == requestedState)
            return;

        if (currentState == State.Advertising)
        {
            this.StopAdvertising();
        }

        if (currentState == State.Discovering)
        {
            this.StopDiscovering();
        }

        if (requestedState == State.Idle)
        {
            this.StopAdvertising();
            this.StopDiscovering();
        }

        if (requestedState == State.Advertising)
        {
            this.StartAdvertising();
        }

        if (requestedState == State.Discovering)
        {
            this.StartDiscovering();
        }
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo) => this.AcceptConnection(endpoint);

    protected override void OnConnectionFailed(EndPoint endpoint) => this.StartDiscovering();

    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        //We found an advertiser!
        this.StopDiscovering();
        this.ConnectToEndpoint(endpoint);
    }
}

public class ChessBottomSheet(IChessActivity activity)
{
    public void Show(IPlayer winner, IPlayer loser)
    {
        activity.GameOverLayout!.Visibility = ViewStates.Visible;
        activity.StandardBottomSheet!.Visibility = ViewStates.Visible;
        activity.MatchmakingLayout!.Visibility = ViewStates.Gone;
        activity.BottomSheet!.AddBottomSheetCallback(activity.Callback!);
        activity.BottomSheet!.State = BottomSheetBehavior.StateHalfExpanded;

        activity.WinnerUsername!.Text = winner.Name;
        activity.WinnerDescription!.Text = $"{winner.Name} Wins, {loser.Name} loses";
        switch (winner)
        {
            case Black:
                activity.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                activity.Indicator?.SetImageResource(Resource.Drawable.king_black);
                break;
            default:
                activity.WinningPlayer?.SetImageDrawable(activity.WhitePlayerProfilePicture?.Drawable);
                break;
        }

        activity.Home!.Click += (_, _) => activity.Finish();
    }
}
