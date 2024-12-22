using Android;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby;
using Android.Gms.Nearby.Connection;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Chess.Util.Logger;
using Java.Util;
using KeySet = System.Collections.Generic.Dictionary<string, EndPoint>.KeyCollection;
using ValueSet = System.Collections.Generic.Dictionary<string, EndPoint>.ValueCollection;

//todo rename all var a to a meaningful name 
public abstract class ConnectionsActivity : AppCompatActivity
{
    public bool IsConnecting { get; private set; } = false;
    public bool IsDiscovering { get; private set; } = false;
    public bool IsAdvertising { get; private set; } = false;
    public readonly Dictionary<string, EndPoint> EstablishedConnections = new();
    public readonly Dictionary<string, EndPoint> PendingConnections = new();
    public readonly Dictionary<string, EndPoint> DiscoveredEndpoints = new();
    private const int RequestCodeRequiredPermissions = 1;
    internal IConnectionsClient ConnectionsClient;
    private string[] requiredPermissions;
    private List<string> requestPermissions;

    internal class ConnectionLifecycleCallback : Android.Gms.Nearby.Connection.ConnectionLifecycleCallback
    {
        private ConnectionsActivity instance;
        public ConnectionLifecycleCallback(ConnectionsActivity instance) : base()
        {
            this.instance = instance;
        }

        public override void OnConnectionInitiated(string endpointId, ConnectionInfo connectionInfo)
        {
            Log.Debug($"OnConnectionInitiated({nameof(endpointId)}={endpointId}," +
                $" {nameof(connectionInfo.EndpointName)}={connectionInfo.EndpointName})");
            EndPoint endPoint = new(endpointId, connectionInfo.EndpointName);
            this.instance.PendingConnections.Add(endpointId, endPoint);
            this.instance.OnConnectionInitiated(endPoint, connectionInfo);
        }

        public override void OnConnectionResult(string endpointId, ConnectionResolution result)
        {
            Log.Debug($"OnConnectionResponse({nameof(endpointId)}={endpointId}, {nameof(result)}={result})");
            this.instance.IsConnecting = false;
            if (!result.Status.IsSuccess)
            {
                Log.Warn($"Connection failed. Received status {this.instance.ToString(result.Status)}");
                var a = this.instance.PendingConnections[endpointId];
                this.instance.PendingConnections.Remove(endpointId);
                this.instance.OnConnectionFailed(a);
                return;
            }
            this.instance.ConnectedToEndpoint(new(this.instance.PendingConnections[endpointId]));
            this.instance.PendingConnections.Remove(endpointId);
        }

        public override void OnDisconnected(string endpointId)
        {
            if (!this.instance.EstablishedConnections.ContainsKey(endpointId))
            {
                Log.Warn("Unexpected disconnection from endpoint " + endpointId);
                return;
            }
            this.instance.DisconnectedFromEndpoint(this.instance.EstablishedConnections[endpointId]);
        }
    }

    internal class PayloadCallback : Android.Gms.Nearby.Connection.PayloadCallback
    {
        private ConnectionsActivity instance;
        public PayloadCallback(ConnectionsActivity instance) : base()
        {
            this.instance = instance;
        }

        public override void OnPayloadReceived(string endpointId, Payload payload)
        {
            Log.Debug($"OnPayloadReceived(endpointId={endpointId}, payload={payload}");
            this.instance.OnReceive(this.instance.EstablishedConnections[endpointId], payload);
        }

        public override void OnPayloadTransferUpdate(string endpointId, PayloadTransferUpdate update)
        {
            Log.Debug($"OnPayloadTransferUpdate(endpointId={endpointId}, update={update}");
        }
    };

    internal class EndpointDiscoveryCallback : Android.Gms.Nearby.Connection.EndpointDiscoveryCallback
    {
        private ConnectionsActivity instance;
        public EndpointDiscoveryCallback(ConnectionsActivity instance) : base()
        {
            this.instance = instance;
        }

        public override void OnEndpointFound(string endpointId, DiscoveredEndpointInfo info)
        {
            Log.Debug($"OnEndpointFound(endpointId={endpointId}, serviceId={info.ServiceId}, endpointName={info.EndpointName})");
            if (this.instance.GetServiceId().Equals(info.ServiceId))
            {
                EndPoint endpoint = new EndPoint(endpointId, info.EndpointName);
                this.instance.DiscoveredEndpoints.Add(endpointId, endpoint);
                this.instance.OnEndpointDiscovered(endpoint);
            }
        }

        public override void OnEndpointLost(string endpointId)
        {
            Log.Debug($"OnEndpointLost(endpointId={endpointId})");
        }
    }

    public enum State
    {
        Unknown,
        Searching,
        Connected
    }

    /** Called when our Activity is first created. */
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ConnectionsClient = NearbyClass.GetConnectionsClient(this);
        ConnectionsClient.StopDiscovery();
        ConnectionsClient.StopAdvertising();
        ConnectionsClient.StopAllEndpoints();
        requiredPermissions = GetRequiredPermissions();
        requestPermissions = new List<string>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    /** Called when our Activity has been made visible to the user. */

    protected override void OnStart()
    {
        this.requiredPermissions = GetRequiredPermissions();
        if (this.HasPermissions())
        {
            base.OnStart();
            return;
        }

        switch (Build.VERSION.SdkInt)
        {
            case < BuildVersionCodes.M:
                ActivityCompat.RequestPermissions(this, requestPermissions.ToArray(), RequestCodeRequiredPermissions);
                break;
            default:
                base.RequestPermissions(requiredPermissions.ToArray(), RequestCodeRequiredPermissions);
                break;
        }
        base.OnStart();
    }



    private bool HasPermissions()
    {
        bool returnValue = true;
        foreach (string requiredPermission in this.requiredPermissions)
        {
            if (ContextCompat.CheckSelfPermission(this, requiredPermission) != Permission.Granted)
            {
                requestPermissions.Add(requiredPermission);
                returnValue = false;
            }
        }
        return returnValue;
    }

    /** Called when the user has accepted (or denied) our permission request. */
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        if (requestCode == RequestCodeRequiredPermissions)
        {
            int i = 0;
            foreach (var grantResult in grantResults)
            {
                if (grantResult == Permission.Denied)
                {
                    Log.Warn("Failed to request the permission " + permissions[i]);
                    Toast.MakeText(this, "string.error_missing_permissions", ToastLength.Long).Show();
                    Finish();
                    return;
                }
                i++;
            }
            base.OnStart();
        }

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    /**
     * Sets the device to advertising mode. It will broadcast to other devices in discovery mode.
     * Either {@link #onAdvertisingStarted()} or {@link #onAdvertisingFailed()} will be called once
     * we've found out if we successfully entered this mode.
     */
    protected async void StartAdvertising()
    {
        IsAdvertising = true;
        string localEndpointName = GetName();

        AdvertisingOptions.Builder advertisingOptions = new AdvertisingOptions.Builder();
        advertisingOptions.SetStrategy(GetStrategy());

        var a = ConnectionsClient.StartAdvertisingAsync(localEndpointName, GetServiceId(),
            new ConnectionLifecycleCallback(this), advertisingOptions.Build());
        await a;
        if (a.IsCompletedSuccessfully)
        {
            Log.Verbose("Now advertising endpoint " + localEndpointName);
            OnAdvertisingStarted();
        }
        if (a.IsFaulted)
        {
            IsAdvertising = false;
            Log.Warn($"startAdvertising() failed. {a.Exception}");
            OnAdvertisingFailed();
        }
    }

    /** Stops advertising. */
    protected void StopAdvertising()
    {
        IsAdvertising = false;
        ConnectionsClient.StopAdvertising();
    }

    /** Called when advertising successfully starts. Override this method to act on the event. */
    protected virtual void OnAdvertisingStarted() { }

    /** Called when advertising fails to start. Override this method to act on the event. */
    protected virtual void OnAdvertisingFailed() { }

    /**
     * Called when a pending connection with a remote endpoint is created. Use {@link ConnectionInfo}
     * for metadata about the connection (like incoming vs outgoing, or the authentication token). If
     * we want to continue with the connection, call {@link #acceptConnection(Endpoint)}. Otherwise,
     * call {@link #rejectConnection(Endpoint)}.
     */
    protected abstract void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo);

    /** Accepts a connection request. */
    protected async void AcceptConnection(EndPoint endpoint)
    {
        var a = ConnectionsClient.AcceptConnectionAsync(endpoint.id, new PayloadCallback(this));
        await a;
        if (a.IsFaulted)
        {
            Log.Warn($"AcceptConnection() failed. {a.Exception}");
        }
    }

    /** Rejects a connection request. */
    protected async void RejectConnection(EndPoint endpoint)
    {
        var a = ConnectionsClient.RejectConnectionAsync(endpoint.id);
        await a;
        if (a.IsFaulted)
        {
            Log.Warn($"RejectConnection() failed. {a.Exception}");
        }
    }

    /**
     * Sets the device to discovery mode. It will now listen for devices in advertising mode. Either
     * {@link #onDiscoveryStarted()} or {@link #onDiscoveryFailed()} will be called once we've found
     * out if we successfully entered this mode.
     */
    public async void StartDiscovering()
    {
        IsDiscovering = true;
        DiscoveredEndpoints.Clear();
        DiscoveryOptions.Builder discoveryOptions = new DiscoveryOptions.Builder();
        discoveryOptions.SetStrategy(GetStrategy());
        var a = ConnectionsClient.StartDiscoveryAsync(GetServiceId(), new EndpointDiscoveryCallback(this), discoveryOptions.Build());
        await a;
        if (a.IsCompletedSuccessfully)
        {
            OnDiscoveryStarted();
        }
        if (a.IsFaulted)
        {
            IsDiscovering = false;
            Log.Warn($"startDiscovering() failed. {a.Exception}");
            OnDiscoveryFailed();
        }
    }

    /** Stops discovery. */
    protected void StopDiscovering()
    {
        IsDiscovering = false;
        ConnectionsClient.StopDiscovery();
    }

    /** Called when discovery successfully starts. Override this method to act on the event. */
    protected virtual void OnDiscoveryStarted() { }

    /** Called when discovery fails to start. Override this method to act on the event. */
    protected virtual void OnDiscoveryFailed() { }

    /**
     * Called when a remote endpoint is discovered. To connect to the device, call {@link
     * #connectToEndpoint(Endpoint)}.
     */
    protected abstract void OnEndpointDiscovered(EndPoint endpoint);

    /** Disconnects from the given endpoint. */
    protected void Disconnect(EndPoint endpoint)
    {
        ConnectionsClient.DisconnectFromEndpoint(endpoint.id);
        EstablishedConnections.Remove(endpoint.id);
    }

    /** Disconnects from all currently connected endpoints. */
    protected void DisconnectFromAllEndpoints()
    {
        foreach (EndPoint endpoint in EstablishedConnections.Values)
        {
            ConnectionsClient.DisconnectFromEndpoint(endpoint.id);
        }
        EstablishedConnections.Clear();
    }

    /** Resets and clears all state in Nearby Connections. */
    protected void StopAllEndpoints()
    {
        this.ConnectionsClient.StopAllEndpoints();
        IsAdvertising = false;
        IsDiscovering = false;
        IsConnecting = false;
        DiscoveredEndpoints.Clear();
        PendingConnections.Clear();
        EstablishedConnections.Clear();
    }

    /**
     * Sends a connection request to the endpoint. Either {@link #onConnectionInitiated(Endpoint,
     * ConnectionInfo)} or {@link #onConnectionFailed(Endpoint)} will be called once we've found out
     * if we successfully reached the device.
     */
    protected async void ConnectToEndpoint(EndPoint endpoint)
    {
        Log.Verbose("Sending a connection request to endpoint " + endpoint);
        // Mark ourselves as connecting so we don't connect multiple times
        IsConnecting = true;

        // Ask to connect
        var a = ConnectionsClient.RequestConnectionAsync(GetName(), endpoint.id, new ConnectionLifecycleCallback(this));
        await a;
        if (a.IsFaulted)
        {
            Log.Warn($"RequestConnection() failed. {a.Exception}");
            IsConnecting = false;
            OnConnectionFailed(endpoint);
            return;
        }
    }

    internal void ConnectedToEndpoint(EndPoint endpoint)
    {
        Log.Debug($"connectedToEndpoint(endpoint={endpoint})");
        EstablishedConnections.Add(endpoint.id, endpoint);
        OnEndpointConnected(endpoint);
    }

    internal void DisconnectedFromEndpoint(EndPoint endpoint)
    {
        Log.Debug($"disconnectedFromEndpoint(endpoint={endpoint})");
        EstablishedConnections.Remove(endpoint.id);
        OnEndpointDisconnected(endpoint);
    }

    /**
     * Called when a connection with this endpoint has failed. Override this method to act on the
     * event.
     */
    protected abstract void OnConnectionFailed(EndPoint endpoint);

    /** Called when someone has connected to us. Override this method to act on the event. */
    protected abstract void OnEndpointConnected(EndPoint endpoint);

    /** Called when someone has disconnected. Override this method to act on the event. */
    protected abstract void OnEndpointDisconnected(EndPoint endpoint);

    /** Returns a list of currently connected endpoints. */
    protected ValueSet GetDiscoveredEndpoints()
    {
        return DiscoveredEndpoints.Values;
    }

    /** Returns a list of currently connected endpoints. */
    protected ValueSet GetConnectedEndpoints()
    {
        return EstablishedConnections.Values;
    }

    /**
     * Sends a {@link Payload} to all currently connected endpoints.
     *
     * @param payload The data you want to send.
     */
    protected void Send(Payload payload)
    {
        Send(payload, EstablishedConnections.Keys);
    }

    private async void Send(Payload payload, KeySet endpoints)
    {
        var a = ConnectionsClient
                .SendPayloadAsync(new List<string>(endpoints), payload);
        await a;
        if (a.IsFaulted)
        {
            Log.Warn($"sendPayload() failed. {a.Exception}");
        }
    }

    /**
     * Someone connected to us has sent us data. Override this method to act on the event.
     *
     * @param endpoint The sender.
     * @param payload The data.
     */
    protected abstract void OnReceive(EndPoint endpoint, Payload payload);

    /**
     * An optional hook to pool any permissions the app needs with the permissions ConnectionsActivity
     * will request.
     *
     * @return All permissions required for the app to properly function.
     */
    protected virtual string[] GetRequiredPermissions()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            return new string[] {
            Manifest.Permission.BluetoothScan,
            Manifest.Permission.BluetoothAdvertise,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.NearbyWifiDevices,
            };

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            return new string[] {
            Manifest.Permission.BluetoothScan,
            Manifest.Permission.BluetoothAdvertise,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            };

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            return new string[] {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            };

        return new string[] {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
        };

    }

    /** Returns the client's name. Visible to others when connecting. */
    protected abstract string GetName();

    /**
     * Returns the service id. This represents the action this connection is for. When discovering,
     * we'll verify that the advertiser has the same service id before we consider connecting to them.
     */
    protected abstract string GetServiceId();

    /**
     * Returns the strategy we use to connect to other devices. Only devices using the same strategy
     * and service id will appear when discovering. Stragies determine how many incoming and outgoing
     * connections are possible at the same time, as well as how much bandwidth is available for use.
     */
    protected abstract Strategy GetStrategy();

    /**
     * Transforms a {@link Status} into a English-readable message for logging.
     *
     * @param status The current status
     * @return A readable String. eg. [404]File not found.
     */

    private string ToString(Statuses status)
    {
        return Java.Lang.String.Format(Locale.Us,
            "[%d]%s",
            status.StatusCode,
            status.StatusMessage != null ? status.StatusMessage
                : ConnectionsStatusCodes.GetStatusCodeString(status.StatusCode)).ToString();
    }

}

public class EndPoint
{
    public readonly string id;
    public readonly string name;

    public EndPoint(string id, string name)
    {
        this.id = id;
        this.name = name;
        Log.Debug($"Endpoint created: {this.ToString()}");
    }

    public EndPoint(EndPoint other)
    {
        this.id = other.id;
        this.name = other.name;
    }

    public override int GetHashCode()
    {
        return this.id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is EndPoint other)
            return this.id.Equals(other);

        return false;
    }

    public override string ToString()
    {
        return $"{nameof(EndPoint)}{{id={id}, name={name}}}";
    }
}