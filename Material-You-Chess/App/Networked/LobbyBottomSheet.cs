using Android.Gms.Nearby.Connection;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Common.Extensions;
using Chess.App.Common.Permissions;
using Chess.App.Nearby;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.ProgressIndicator;

namespace Chess.App.Networked;

public abstract class LobbyBottomSheet : ConnectionsActivity
{
    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }
    public Callback? Callback { get; set; }
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
        this.Callback = new Callback(this);
        this.StandardBottomSheet = base.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet);
        this.BottomSheetLayout = base.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet);
        this.BottomSheet = BottomSheetBehavior.From(this.BottomSheetLayout!);
        this.BottomSheet!.AddBottomSheetCallback(this.Callback);
        this.BottomSheet!.State = BottomSheetBehavior.StateHalfExpanded;

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
