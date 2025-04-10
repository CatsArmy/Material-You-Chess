using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common.Extensions;
using Chess.App.Common.Permissions;
using Chess.Game.Common;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.ProgressIndicator;

namespace Chess.App.Networked.Nearby;

public partial class NetworkedChessActivity : ConnectionsActivity
{
    private NearbyConnections? PermissionManager;

    public ChessBottomSheet? ChessBottomSheet { get; set; }
    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }
    public ConstraintLayout? GameOverLayout { get; set; }
    public ConstraintLayout? MatchmakingLayout { get; set; }
    public BottomSheetCallback? Callback { get; set; }

    public TextView? SearchingText { get; set; }
    public CircularProgressIndicator? SearchingIndicator { get; set; }
    public ChipGroup? MatchmakingPreferences { get; set; }
    public Chip? White { get; set; }
    public Chip? Black { get; set; }

    private ConnectionsClientState State
    {
        get; set
        {
            this.OnStateChanged(field, value);
            field = value;
        }
    } = ConnectionsClientState.Idle;

    public virtual void OnSelectNone()
    {
        this.SearchingIndicator?.Hide();
        this.SearchingText!.Text = "Please select a matchmaking preference";
        this.State = ConnectionsClientState.Idle;
    }

    public virtual void OnSelectWhite()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
        this.State = ConnectionsClientState.Advertising;
    }

    public virtual void OnSelectBlack()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
        this.State = ConnectionsClientState.Discovering;
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

    private void OnStateChanged(ConnectionsClientState currentState, ConnectionsClientState requestedState)
    {
        if (currentState == requestedState)
            return;

        if (currentState == ConnectionsClientState.Advertising)
        {
            this.StopAdvertising();
        }

        if (currentState == ConnectionsClientState.Discovering)
        {
            this.StopDiscovering();
        }

        if (requestedState == ConnectionsClientState.Idle)
        {
            this.StopAdvertising();
            this.StopDiscovering();
        }

        if (requestedState == ConnectionsClientState.Advertising)
        {
            this.StartAdvertising();
        }

        if (requestedState == ConnectionsClientState.Discovering)
        {
            this.StartDiscovering();
        }
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo) => this.AcceptConnection(endpoint);

    protected override void OnConnectionFailed(EndPoint endpoint) => this.StartDiscovering();

    /// <summary> We found an advertiser! </summary>
    /// <param name="endpoint">the endpoint of the advertiser we discovered</param>
    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        this.StopDiscovering();
        this.ConnectToEndpoint(endpoint);
    }

    private enum ConnectionsClientState
    {
        Idle,
        Advertising,
        Discovering
    }
}
