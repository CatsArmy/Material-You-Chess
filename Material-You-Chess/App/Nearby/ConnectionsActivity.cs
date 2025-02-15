using System.Diagnostics.CodeAnalysis;
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
using Chess.App.Common;
using Java.Util;
using KeySet = System.Collections.Generic.Dictionary<string, Chess.App.Nearby.EndPoint>.KeyCollection;
using ValueSet = System.Collections.Generic.Dictionary<string, Chess.App.Nearby.EndPoint>.ValueCollection;

namespace Chess.App.Nearby;

public abstract class ConnectionsActivity : AppCompatActivity
{
    public bool IsConnecting { get; private set; } = false;
    public bool IsDiscovering { get; private set; } = false;
    public bool IsAdvertising { get; private set; } = false;
    public readonly Dictionary<string, EndPoint> EstablishedConnections = [];
    public readonly Dictionary<string, EndPoint> PendingConnections = [];
    public readonly Dictionary<string, EndPoint> DiscoveredEndpoints = [];
    private const int RequestCodeRequiredPermissions = 1;
    internal IConnectionsClient? ConnectionsClient;
    private string[]? requiredPermissions;
    private List<string>? requestPermissions;

    internal class ConnectionLifecycleCallback(ConnectionsActivity instance) : Android.Gms.Nearby.Connection.ConnectionLifecycleCallback()
    {
        public override void OnConnectionInitiated(string endpointId, ConnectionInfo connectionInfo)
        {
            Logger.Debug($"OnConnectionInitiated({nameof(endpointId)}={endpointId}," +
                $" {nameof(connectionInfo.EndpointName)}={connectionInfo.EndpointName})");
            var endPoint = new EndPoint(endpointId, connectionInfo.EndpointName);
            instance.PendingConnections.Add(endpointId, endPoint);
            instance.OnConnectionInitiated(endPoint, connectionInfo);
        }

        public override void OnConnectionResult(string endpointId, ConnectionResolution result)
        {
            Logger.Debug($"OnConnectionResponse({nameof(endpointId)}={endpointId}, {nameof(result)}={result})");
            instance.IsConnecting = false;
            if (!result.Status.IsSuccess)
            {
                Logger.Warn($"Connection failed. Received status {ConnectionsActivity.ToString(result.Status)}");
                EndPoint? failed = instance.PendingConnections[endpointId];
                instance.PendingConnections.Remove(endpointId);
                instance.OnConnectionFailed(failed);
                return;
            }
            instance.ConnectedToEndpoint(new(instance.PendingConnections[endpointId]));
            instance.PendingConnections.Remove(endpointId);
        }

        public override void OnDisconnected(string endpointId)
        {
            if (!instance.EstablishedConnections.TryGetValue(endpointId, out EndPoint? connection))
            {
                Logger.Warn("Unexpected disconnection from endpoint " + endpointId);
                return;
            }
            instance.DisconnectedFromEndpoint(connection);
        }
    }

    internal class PayloadCallback(ConnectionsActivity instance) : Android.Gms.Nearby.Connection.PayloadCallback()
    {
        private readonly ConnectionsActivity instance = instance;

        public override void OnPayloadReceived(string endpointId, Payload payload)
        {
            Logger.Debug($"OnPayloadReceived(endpointId={endpointId}, payload={payload}");
            this.instance.OnReceive(this.instance.EstablishedConnections[endpointId], payload);
        }

        public override void OnPayloadTransferUpdate(string endpointId, PayloadTransferUpdate update)
        {
            Logger.Debug($"OnPayloadTransferUpdate(endpointId={endpointId}, update={update}");
        }
    };

    internal class EndpointDiscoveryCallback(ConnectionsActivity instance) : Android.Gms.Nearby.Connection.EndpointDiscoveryCallback()
    {
        private readonly ConnectionsActivity instance = instance;

        public override void OnEndpointFound(string endpointId, DiscoveredEndpointInfo info)
        {
            Logger.Debug($"OnEndpointFound(endpointId={endpointId}, serviceId={info.ServiceId}, endpointName={info.EndpointName})");
            if (this.instance.ServiceId.Equals(info.ServiceId))
            {
                EndPoint endpoint = new(endpointId, info.EndpointName);
                this.instance.DiscoveredEndpoints.Add(endpointId, endpoint);
                this.instance.OnEndpointDiscovered(endpoint);
            }
        }

        public override void OnEndpointLost(string endpointId)
        {
            Logger.Debug($"OnEndpointLost(endpointId={endpointId})");
        }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.ConnectionsClient = NearbyClass.GetConnectionsClient(this);
        this.ConnectionsClient.StopDiscovery();
        this.ConnectionsClient.StopAdvertising();
        this.ConnectionsClient.StopAllEndpoints();
        this.requiredPermissions = GetRequiredPermissions();
        this.requestPermissions = [];
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// Called when our Activity has been made visible to the user.
    /// </summary>
    protected override void OnStart()
    {
        this.requiredPermissions = this.GetRequiredPermissions();
        if (this.HasPermissions())
        {
            base.OnStart();
            return;
        }

        switch (Build.VERSION.SdkInt)
        {
            case < BuildVersionCodes.M:
                ActivityCompat.RequestPermissions(this, [.. requestPermissions!], RequestCodeRequiredPermissions);
                break;
            default:
                base.RequestPermissions([.. this.requiredPermissions], RequestCodeRequiredPermissions);
                break;
        }
        base.OnStart();
    }

    private bool HasPermissions()
    {
        bool returnValue = true;
        foreach (string requiredPermission in this.requiredPermissions!)
        {
            if (ContextCompat.CheckSelfPermission(this, requiredPermission) != Permission.Granted)
            {
                this.requestPermissions!.Add(requiredPermission);
                returnValue = false;
            }
        }
        return returnValue;
    }

    /// <summary>
    /// Called when the user has accepted (or denied) our permission request.
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="permissions"></param>
    /// <param name="grantResults"></param>
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        if (requestCode == RequestCodeRequiredPermissions)
        {
            int i = 0;
            foreach (var grantResult in grantResults)
            {
                if (grantResult == Permission.Denied)
                {
                    Logger.Warn("Failed to request the permission " + permissions[i]);
                    Toast.MakeText(this, "string.error_missing_permissions", ToastLength.Long)?.Show();
                    Finish();
                    return;
                }
                i++;
            }
            base.OnStart();
        }

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    /// <summary>
    /// Sets the device to advertising mode. It will broadcast to other devices in discovery mode.
    /// Either <see cref="OnAdvertisingStarted()"/> or <see cref="OnAdvertisingFailed()"/> will be called once
    /// we've found out if we successfully entered this mode.
    /// </summary>
    protected async void StartAdvertising()
    {
        this.IsAdvertising = true;
        string localEndpointName = Name;

        var options = new AdvertisingOptions.Builder();
        options.SetStrategy(this.Strategy);

        Task? advertising = this.ConnectionsClient!.StartAdvertisingAsync(localEndpointName, this.ServiceId, new ConnectionLifecycleCallback(this),
            options.Build());
        await advertising;
        if (advertising.IsCompletedSuccessfully)
        {
            Logger.Verbose("Now advertising endpoint " + localEndpointName);
            this.OnAdvertisingStarted();
        }
        if (advertising.IsFaulted)
        {
            this.IsAdvertising = false;
            Logger.Warn($"startAdvertising() failed. {advertising.Exception}");
            this.OnAdvertisingFailed();
        }
    }

    /// <summary>
    /// Stops advertising.
    /// </summary>
    protected void StopAdvertising()
    {
        this.IsAdvertising = false;
        this.ConnectionsClient!.StopAdvertising();
    }

    /// <summary>
    /// Called when advertising successfully starts. Override this method to act on the event.
    /// </summary>
    protected virtual void OnAdvertisingStarted() { }

    /// <summary>
    /// Called when advertising fails to start. Override this method to act on the event.
    /// </summary>
    protected virtual void OnAdvertisingFailed() { }

    /// <summary>
    /// Called when a pending connection with a remote endpoint is created. Use <see cref="ConnectionInfo"/>
    /// for metadata about the connection (like incoming vs outgoing, or the authentication token). If
    /// we want to continue with the connection, call <see cref="AcceptConnection(EndPoint)"/>. Otherwise,
    /// call <see cref="RejectConnection(EndPoint)"/>.
    /// </summary>

    protected abstract void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo);

    /// <summary>
    /// Accepts a connection request.
    /// </summary>
    /// <param name="endpoint"></param>
    protected async void AcceptConnection(EndPoint endpoint)
    {
        Task? accept = this.ConnectionsClient!.AcceptConnectionAsync(endpoint.Id, new PayloadCallback(this));
        await accept;
        if (accept.IsFaulted)
        {
            Logger.Warn($"{this.AcceptConnection}() failed. {accept.Exception}");
        }
    }

    /// <summary>
    /// Rejects a connection request.
    /// </summary>
    /// <param name="endpoint"></param>
    protected async void RejectConnection(EndPoint endpoint)
    {
        var reject = this.ConnectionsClient!.RejectConnectionAsync(endpoint.Id);
        await reject;
        if (reject.IsFaulted)
        {
            Logger.Warn($"{this.RejectConnection}() failed. {reject.Exception}");
        }
    }

    /// <summary>
    /// Sets the device to discovery mode. It will now listen for devices in advertising mode.
    /// Either <see cref="OnDiscoveryStarted()"/> or <see cref="OnDiscoveryFailed()"/> will be called once we've found
    /// out if we successfully entered this mode.
    /// </summary>

    public async void StartDiscovering()
    {
        this.IsDiscovering = true;
        this.DiscoveredEndpoints.Clear();
        DiscoveryOptions.Builder discoveryOptions = new DiscoveryOptions.Builder().SetStrategy(this.Strategy);
        Task? discovery = this.ConnectionsClient!.StartDiscoveryAsync(this.ServiceId, new EndpointDiscoveryCallback(this), discoveryOptions.Build());
        await discovery;
        if (discovery.IsCompletedSuccessfully)
        {
            this.OnDiscoveryStarted();
        }
        if (discovery.IsFaulted)
        {
            this.IsDiscovering = false;
            Logger.Warn($"{this.StartDiscovering}() failed. {discovery.Exception}");
            this.OnDiscoveryFailed();
        }
    }

    /** Stops discovery. */
    protected void StopDiscovering()
    {
        this.IsDiscovering = false;
        this.ConnectionsClient!.StopDiscovery();
    }

    /// <summary>
    /// Called when discovery successfully starts. Override this method to act on the event.
    /// </summary>
    protected virtual void OnDiscoveryStarted() { }

    /// <summary>
    /// Called when discovery fails to start. Override this method to act on the event.
    /// </summary>
    protected virtual void OnDiscoveryFailed() { }

    ///<summary>
    ///Called when a remote endpoint is discovered. To connect to the device, call <see cref="ConnectToEndpoint(EndPoint)"/>.
    ///</summary>
    protected abstract void OnEndpointDiscovered(EndPoint endpoint);

    /// <summary>
    /// Disconnects from the given endpoint.
    /// </summary>
    /// <param name="endpoint"></param>
    protected void Disconnect(EndPoint endpoint)
    {
        this.ConnectionsClient!.DisconnectFromEndpoint(endpoint.Id);
        this.EstablishedConnections.Remove(endpoint.Id);
    }

    /// <summary>
    /// Disconnects from all currently connected endpoints.
    /// </summary>
    protected void DisconnectFromAllEndpoints()
    {
        foreach (EndPoint endpoint in this.EstablishedConnections.Values)
        {
            this.ConnectionsClient!.DisconnectFromEndpoint(endpoint.Id);
        }
        this.EstablishedConnections.Clear();
    }

    /// <summary>
    /// Resets and clears all state in Nearby Connections.
    /// </summary>
    protected void StopAllEndpoints()
    {
        this.ConnectionsClient!.StopAllEndpoints();
        this.IsAdvertising = false;
        this.IsDiscovering = false;
        this.IsConnecting = false;
        this.DiscoveredEndpoints.Clear();
        this.PendingConnections.Clear();
        this.EstablishedConnections.Clear();
    }


    /// <summary>
    /// Sends a connection request to the endpoint. Either <see cref="OnConnectionInitiated(EndPoint, ConnectionInfo)"/> or 
    /// <see cref="OnConnectionFailed(EndPoint)"/> will be called once we've found out if we successfully reached the device.
    /// </summary>
    /// <param name="endpoint"></param>
    protected async void ConnectToEndpoint(EndPoint endpoint)
    {
        Logger.Verbose($"Sending a connection request to endpoint {endpoint}");
        // Mark ourselves as connecting so we don't connect multiple times
        this.IsConnecting = true;

        // Ask to connect
        Task? connection = this.ConnectionsClient!.RequestConnectionAsync(this.Name, endpoint.Id, new ConnectionLifecycleCallback(this));
        await connection;
        if (connection.IsFaulted)
        {
            Logger.Warn($"RequestConnection() failed. {connection.Exception}");
            this.IsConnecting = false;
            this.OnConnectionFailed(endpoint);
            return;
        }
    }

    internal void ConnectedToEndpoint(EndPoint endpoint)
    {
        Logger.Debug($"connectedToEndpoint(endpoint={endpoint})");
        this.EstablishedConnections.Add(endpoint.Id, endpoint);
        this.OnEndpointConnected(endpoint);
    }

    internal void DisconnectedFromEndpoint(EndPoint endpoint)
    {
        Logger.Debug($"disconnectedFromEndpoint(endpoint={endpoint})");
        this.EstablishedConnections.Remove(endpoint.Id);
        this.OnEndpointDisconnected(endpoint);
    }


    /// <summary>
    /// Called when a connection with this endpoint has failed. Override this method to act on the event.
    /// </summary>
    /// <param name="endpoint"></param>
    protected abstract void OnConnectionFailed(EndPoint endpoint);

    /// <summary>
    /// Called when someone has connected to us. Override this method to act on the event.
    /// </summary>
    /// <param name="endpoint"></param>
    protected abstract void OnEndpointConnected(EndPoint endpoint);

    /// <summary>
    /// Called when someone has disconnected. Override this method to act on the event.
    /// </summary>
    /// <param name="endpoint"></param>
    protected abstract void OnEndpointDisconnected(EndPoint endpoint);

    /// <summary>
    /// Returns a list of currently connected endpoints.
    /// </summary>
    /// <returns></returns>
    protected ValueSet GetDiscoveredEndpoints()
    {
        return this.DiscoveredEndpoints.Values;
    }

    /// <summary>
    /// Returns a list of currently connected endpoints.
    /// </summary>
    /// <returns></returns>
    protected ValueSet GetConnectedEndpoints()
    {
        return this.EstablishedConnections.Values;
    }


    /// <summary>
    /// Sends a <see cref="Payload"/> to all currently connected endpoints.
    /// </summary>
    /// <param name="payload">The data you want to send.</param>

    protected void Send(Payload payload)
    {
        this.Send(payload, this.EstablishedConnections.Keys);
    }

    private async void Send(Payload payload, KeySet endpoints)
    {
        Task? sent = this.ConnectionsClient!.SendPayloadAsync([.. endpoints], payload);
        await sent;
        if (sent.IsFaulted)
        {
            Logger.Warn($"sendPayload() failed. {sent.Exception}");
        }
    }


    /// <summary>
    ///Someone connected to us has sent us data. Override this method to act on the event.
    /// </summary>
    /// <param name="endpoint">endpoint The sender.</param>
    /// <param name="payload">payload The data.</param>
    protected abstract void OnReceive(EndPoint endpoint, Payload payload);


    /// <summary>
    /// An optional hook to pool any permissions the app needs with the permissions ConnectionsActivity will request.
    /// </summary>
    /// <returns> All permissions required for the app to properly function.</returns>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
       Justification = "Only when version is 33 or above")]
    protected virtual string[] GetRequiredPermissions()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            return [
            Manifest.Permission.BluetoothScan,
            Manifest.Permission.BluetoothAdvertise,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.NearbyWifiDevices,
            ];

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            return [
            Manifest.Permission.BluetoothScan,
            Manifest.Permission.BluetoothAdvertise,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            ];

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            return [
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            ];

        return [
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.ChangeWifiState,
            Manifest.Permission.AccessCoarseLocation,
        ];

    }

    /// <summary>
    /// Returns the client's name. Visible to others when connecting.
    /// </summary>
    protected abstract string Name { get; }

    /// <summary>
    /// Returns the service id. This represents the action this connection is for. When discovering,
    /// we'll verify that the advertiser has the same service id before we consider connecting to them.
    /// </summary>
    protected abstract string ServiceId { get; }


    /// <summary>
    /// Returns the strategy we use to connect to other devices. 
    /// Only devices using the same strategy and service id will appear when discovering. 
    /// Strategies determine how many incoming and outgoing connections are possible at the same time,
    /// as well as how much bandwidth is available for use.
    /// </summary>

    protected abstract Strategy Strategy { get; }


    /// <summary>
    /// Transforms a <see cref="Statuses"/> into a English-readable message for logging.
    /// </summary>
    /// <param name="status">The current status</param>
    /// <returns> A readable String.eg. [404] File not found.</returns>

    private static string ToString(Statuses status)
    {
        string msg = (status.StatusMessage == null) switch
        {
            true => ConnectionsStatusCodes.GetStatusCodeString(status.StatusCode),
            false => status.StatusMessage
        };
        return Java.Lang.String.Format(Locale.Us!, "[%d]%s", status.StatusCode, msg!).ToString();
    }

}
