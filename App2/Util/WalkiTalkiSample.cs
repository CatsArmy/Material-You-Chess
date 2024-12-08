using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Gms.Nearby.Connection;
using Android.OS;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using Chess;

[Activity(Label = "@string/app_name", Theme = "@style/Theme.Material3.DynamicColors.DayNight.NoActionBar")]
public class MainActivity2 : ConnectionsActivity
{
    /** If true, debug logs are shown on the device. */
    private const bool DEBUG = true;

    /**
     * The connection strategy we'll use for Nearby Connections. In this case, we've decided on
     * P2P_STAR, which is a combination of Bluetooth Classic and WiFi Hotspots.
     */
    private static readonly Strategy Strategy = Strategy.P2pPointToPoint;

    /**
     * This service id lets us find other nearby devices that are interested in the same thing. Our
     * sample does exactly one thing, so we hardcode the ID.
     */
    private const string ServiceId =
        "com.google.location.nearby.apps.walkietalkie.automatic.SERVICE_ID";

    /**
     * The state of the app. As the app changes states, the UI will update and advertising/discovery
     * will start/stop.
     */
    private State state = State.Unknown;

    /** A random UID used as this device's endpoint name. */
    private string mName;

    /**
     * The background color of the 'CONNECTED' state. This is randomly chosen from the {@link #COLORS}
     * list, based off the authentication token.
     */


    /** An animator that controls the animation from previous state to current state. */


    /** A running log of debug messages. Only visible when DEBUG=true. */
    private TextView mDebugLogView;
    private TextView previousStateView;
    private TextView currentStateView;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        base.SetContentView(Resource.Layout.nearby_template);

        mDebugLogView = base.FindViewById<TextView>(Resource.Id.logView);
        mDebugLogView.Visibility = DEBUG ? ViewStates.Visible : ViewStates.Gone;
        mDebugLogView.MovementMethod = new ScrollingMovementMethod();
        previousStateView = base.FindViewById<TextView>(Resource.Id.oldState);
        currentStateView = base.FindViewById<TextView>(Resource.Id.newState);
        //the username from firebase
        mName = "Guest2";
    }
    protected override void OnStart()
    {
        base.OnStart();
        SetState(State.Searching);
    }


    protected override void OnStop()
    {
        // After our Activity stops, we disconnect from Nearby Connections.
        SetState(State.Unknown);
        base.OnStop();
    }


    public override void OnBackPressed()
    {
        if (state == State.Connected)
        {
            SetState(State.Searching);
            return;
        }
        base.OnBackPressed();
    }

    protected override void OnEndpointDiscovered(EndPoint endpoint)
    {
        // We found an advertiser!
        StopDiscovering();
        ConnectToEndpoint(endpoint);
    }

    protected override void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo)
    {
        // A connection to another device has been initiated! We'll use the auth token, which is the
        // same on both devices, to pick a color to use when we're connected. This way, users can
        // visually see which device they connected with.
        //TODO Noted

        //mConnectedColor = COLORS[connectionInfo.getAuthenticationToken().hashCode() % COLORS.length];

        // We accept the connection immediately.
        AcceptConnection(endpoint);
    }

    protected override void OnEndpointConnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_connected, {endpoint.name}", ToastLength.Short).Show();
        SetState(State.Connected);
    }


    protected override void OnEndpointDisconnected(EndPoint endpoint)
    {
        Toast.MakeText(this, $"Resource.String.toast_disconnected, {endpoint.name}", ToastLength.Short).Show();
        SetState(State.Searching);
    }

    protected override void OnConnectionFailed(EndPoint endpoint)
    {
        // Let's try someone else.
        if (state == State.Searching)
        {
            StartDiscovering();
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
            Log.Warn("CatDebug", "State set to " + state + " but already in that state");
            return;
        }

        Log.Debug("CatDebug", "State set to " + state);
        State oldState = this.state;
        this.state = state;
        OnStateChanged(oldState, state);
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
                DisconnectFromAllEndpoints();
                StartDiscovering();
                StartAdvertising();
                break;
            case State.Connected:
                StopDiscovering();
                StopAdvertising();
                break;
            case State.Unknown:
                StopAllEndpoints();
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
    [UiThread]
    private void UpdateTextView(TextView textView, State state, string textViewName)
    {
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

    /** {@see ConnectionsActivity#onReceive(Endpoint, Payload)} */
    protected override void OnReceive(EndPoint endpoint, Payload payload)
    {
        if (payload.PayloadType == Payload.Type.Bytes)
        {

        }
    }


    /** Starts recording sound from the microphone and streaming it to all connected devices. */
    private void startRecording()
    {
        //logV("startRecording()");
        try
        {
            ParcelFileDescriptor[] payloadPipe = ParcelFileDescriptor.CreatePipe();
            //networking
            //Payload.FromBytes();
            // Send the first half of the payload (the read side) to Nearby Connections.
            Send(Payload.FromStream(payloadPipe[0]));

            // Use the second half of the payload (the write side) in AudioRecorder.
            //mRecorder = new AudioRecorder(payloadPipe[1]);
            //mRecorder.start();
        }
        catch (Exception e)
        {
            Log.Error("CatDebug", $"startRecording() failed {e}");
        }
    }

    /** {@see ConnectionsActivity#getRequiredPermissions()} */
    protected override string[] GetRequiredPermissions()
    {
        var perms = base.GetRequiredPermissions().ToList();
        var newPerms = new List<string>();
        newPerms.AddRange(perms);
        newPerms.Add(Android.Manifest.Permission.RecordAudio);
        return newPerms.ToArray();
    }

    /**
     * Queries the phone's contacts for their own profile, and returns their name. Used when
     * connecting to another device.
     */
    protected override string GetName()
    {
        return mName;
    }

    /** {@see ConnectionsActivity#getServiceId()} */

    protected override string GetServiceId()
    {
        return ServiceId;
    }

    /** {@see ConnectionsActivity#getStrategy()} */

    protected override Strategy GetStrategy()
    {
        return Strategy;
    }

    public enum State
    {
        Unknown,
        Searching,
        Connected
    }
}