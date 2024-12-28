using System.Diagnostics.CodeAnalysis;
using Android.Gms.Nearby.Connection;
using Android.Text.Method;
using Android.Views;
using Chess;
using Chess.Util.Logger;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Material3.DynamicColors.DayNight.NoActionBar")]
public class MainActivity2 : ConnectionsActivity
{
    private const bool DEBUG = true;
    protected override Strategy Strategy => Strategy.P2pCluster;
    protected override string Name => true switch
    {
        true => "Itai",
        false => "Gila",
    };
    protected override string ServiceId => "com.google.location.nearby.apps.chess";
    private State state = State.Unknown;
    private TextView? mDebugLogView;
    private TextView? previousStateView;
    private TextView? currentStateView;
    //private System.Timers.Timer timer;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        base.SetContentView(Resource.Layout.nearby_template);

        this.mDebugLogView = base.FindViewById<TextView>(Resource.Id.logView);
        this.previousStateView = base.FindViewById<TextView>(Resource.Id.oldState);
        this.currentStateView = base.FindViewById<TextView>(Resource.Id.newState);
        //the username from firebase


        /*
        timer.AutoReset = false;
        timer = new Timer(new TimeSpan(0, 0, 15).TotalMilliseconds);
        timer.Elapsed += StopSearching;
        */

        this.mDebugLogView!.Visibility = DEBUG ? ViewStates.Visible : ViewStates.Gone;
        this.mDebugLogView!.MovementMethod = new ScrollingMovementMethod();
        Log.LogView = this.mDebugLogView;
    }

    //private void OnRefresh()
    //{
    //    //clear found devices ^^^
    //    timer.Elapsed -= StopSearching;
    //    timer.Elapsed += (_, _) => StopAllEndpoints();
    //    timer.Stop();
    //    timer.Elapsed -= (_, _) => StopAllEndpoints();
    //    timer.Interval = new TimeSpan(0, 0, 15).TotalMilliseconds;
    //    timer.Elapsed += StopSearching;
    //    timer.Start();
    //}

    //private void StopSearching(object? sender, EventArgs args)
    //{

    //}

    protected override void OnStart()
    {
        base.OnStart();
        this.SetState(State.Searching);
    }


    protected override void OnStop()
    {
        // After our Activity stops, we disconnect from Nearby Connections.
        if (this.state == State.Connected)
            return;

        this.SetState(State.Unknown);
        base.OnStop();
    }

    public override void Finish()
    {
        this.SetState(State.Unknown);
        base.Finish();
    }



    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "<Pending>")]
    public override void OnBackPressed()
    {
        if (this.state == State.Connected)
        {
            this.SetState(State.Searching);
            return;
        }

        base.OnBackPressed();
    }

    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        // We found an advertiser!
        this.StopDiscovering();
        Thread.Sleep(10);
        this.ConnectToEndpoint(endpoint);
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo)
    {
        // A connection to another device has been initiated! We'll use the auth token, which is the
        // same on both devices, to pick a color to use when we're connected. This way, users can
        // visually see which device they connected with.
        //TODO Noted

        //mConnectedColor = COLORS[connectionInfo.getAuthenticationToken().hashCode() % COLORS.length];

        // We accept the connection immediately.
        this.AcceptConnection(endpoint);
    }

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_connected, {endpoint.Name}", ToastLength.Short)?.Show();
        this.SetState(State.Connected);
    }


    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_disconnected, {endpoint.Name}", ToastLength.Short)?.Show();
        this.SetState(State.Searching);
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {
        // Let's try someone else.
        if (this.state == State.Searching)
        {
            this.StartDiscovering();
        }
    }

    /**
     * The state has changed. I wonder what we'll be doing now.
     *
     * @param state The new state.
     */
    private void SetState(State state)
    {
        if (this.state == state)
        {
            Log.Warn($"State set to {state} but already in that state");
            return;
        }

        Log.Debug($"State set to {state}");
        State oldState = this.state;
        this.state = state;
        this.OnStateChanged(oldState, state);
    }

    /**
     * State has changed.
     *
     * @param oldState The previous state we were in. Clean up anything related to this state.
     * @param newState The new state we're now in. Prepare the UI for this state.
     */
    private void OnStateChanged(State oldState, State newState)
    {
        // Update Nearby Connections to the new state.
        switch (newState)
        {
            case State.Searching:
                this.DisconnectFromAllEndpoints();
                this.StartDiscovering();
                this.StartAdvertising();
                break;
            case State.Connected:
                this.StopDiscovering();
                this.StopAdvertising();
                break;
            case State.Unknown:
                this.StopAllEndpoints();
                break;
        }

        // Update the UI.
        switch (oldState)
        {
            case State.Unknown:
                UpdateTextView(previousStateView, oldState, "Previous State:");
                UpdateTextView(currentStateView, newState, "Current State:");
                break;
            case State.Searching:
                switch (newState)
                {
                    case State.Unknown:
                        UpdateTextView(previousStateView, oldState, "Previous State:");
                        UpdateTextView(currentStateView, newState, "Current State:");
                        break;
                    case State.Connected:
                        UpdateTextView(previousStateView, oldState, "Previous State:");
                        UpdateTextView(currentStateView, newState, "Current State:");
                        break;
                }
                break;
            case State.Connected:
                UpdateTextView(previousStateView, oldState, "Previous State:");
                UpdateTextView(currentStateView, newState, "Current State:");
                break;
        }
    }

    /** Updates the {@link TextView} with the correct color/text for the given {@link State}. */
    private static void UpdateTextView(TextView? textView, State state, string textViewName)
    {
        if (textView == null)
            return;

        switch (state)
        {
            case State.Searching:
                textView.Text = $"{textViewName} {nameof(State.Searching)}";
                break;
            case State.Connected:
                textView.Text = $"{textViewName} {nameof(State.Connected)}";
                break;
            case State.Unknown:
                textView.Text = $"{textViewName} {nameof(State.Unknown)}";
                break;
        }
    }

    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {

        }
    }

    protected override string[] GetRequiredPermissions()
    {
        var perms = base.GetRequiredPermissions().ToList();
        var newPerms = new List<string>();
        newPerms.AddRange(perms);
        return newPerms.ToArray();
    }
}

