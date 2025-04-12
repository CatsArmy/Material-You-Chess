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
    public ChessBottomSheet? BottomSheet { get; set; }
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
        this.BottomSheet!.SearchingIndicator?.Hide();
        this.BottomSheet!.SearchingText!.Text = "Please select a matchmaking preference";
        this.State = ConnectionsClientState.Idle;
    }

    public virtual void OnSelectWhite()
    {
        this.BottomSheet!.SearchingIndicator?.Show();
        this.BottomSheet!.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
        this.State = ConnectionsClientState.Advertising;
    }

    public virtual void OnSelectBlack()
    {
        this.BottomSheet!.SearchingIndicator?.Show();
        this.BottomSheet!.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
        this.State = ConnectionsClientState.Discovering;
    }

    /// <param name="isGranted"> <paramref name="isGranted"/> are all of the requested permissions granted </param>
    public override void HandlePermissionResult(bool isGranted)
    {
        if (!isGranted)
        {
            this.SetResult(Result.Canceled);
            this.Finish();
            return;
        }
        //move to main fragment

    }

    public void BindBottomSheet()
    {
        this.PermissionManager?.RequestAccess();
        //move to mainfragment

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
