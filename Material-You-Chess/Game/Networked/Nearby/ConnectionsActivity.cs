using Android.Gms.Nearby;
using Android.Gms.Nearby.Connection;
using AndroidX.AppCompat.App;
using Chess.App.Common;
using Chess.App.Common.Extensions;
using NearbyConnectionLifecycleCallback = Android.Gms.Nearby.Connection.ConnectionLifecycleCallback;
using NearbyEndpointDiscoveryCallback = Android.Gms.Nearby.Connection.EndpointDiscoveryCallback;
using NearbyPayloadCallback = Android.Gms.Nearby.Connection.PayloadCallback;

namespace Chess.Game.Networked.Nearby;

public abstract class ConnectionsActivity : AppCompatActivity
{
    public Dictionary<string, EndPoint> EstablishedConnections { get; private set; } = [];
    public Dictionary<string, EndPoint> PendingConnections { get; private set; } = [];
    public Dictionary<string, EndPoint> DiscoveredEndpoints { get; private set; } = [];
    public bool IsConnected => this.EstablishedConnections.Count > 0;

    public bool IsConnecting
    {
        get; set
        {
            this.SetIsDiscovering(value);
            this.SetIsAdvertising(value);
            field = value;
        }
    } = false;

    public bool IsDiscovering
    {
        get; set
        {
            this.SetIsDiscovering(value);
            field = value;
        }
    } = false;

    public bool IsAdvertising
    {
        get; set
        {
            this.SetIsAdvertising(value);
            field = value;
        }
    } = false;

    /// <summary> The connections client used to advertise/discover nearby devices </summary>
    protected IConnectionsClient? ConnectionsClient;

    /// <summary> Visible to others when connecting. </summary>
    protected abstract string AdvertisingName { get; }

    /// <summary> 
    /// This represents the action this connection is for. When discovering,
    /// we'll verify that the advertiser has the same service id before we consider connecting to them. 
    /// </summary>
    protected abstract string ServiceId { get; }

    /// <summary> 
    /// the strategy we use to connect to other devices. 
    /// Only devices using the same strategy and service id will appear when discovering. 
    /// Strategies determine how many incoming and outgoing connections are possible at the same time,
    /// as well as how much bandwidth is available for use. 
    /// </summary>
    protected abstract Strategy Strategy { get; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        this.ConnectionsClient = NearbyClass.GetConnectionsClient(this);
        this.StopAllEndpoints(disconnect: true); // Cleanup old data if it somehow exists
    }

    protected override void OnDestroy()
    {
        this.StopAllEndpoints(disconnect: true);
        base.OnDestroy();
    }

    private async void SetIsAdvertising(bool value)
    {
        if (this.IsAdvertising == value) return;
        if (this.IsAdvertising) StopAdvertising();
        if (value)
        {
            if (this.IsDiscovering) this.IsDiscovering = false;
            await StartAdvertising();
        }
    }

    private async void SetIsDiscovering(bool value)
    {
        if (this.IsDiscovering == value) return;
        if (this.IsDiscovering) StopDiscovering();
        if (value)
        {
            if (this.IsAdvertising) this.IsAdvertising = false;
            await StartDiscovering();
        }
    }

    /// <summary> Sets the device to advertising mode. It will broadcast to other devices in discovery mode. </summary>
    /// <remarks> Either <see cref="OnAdvertisingStarted()"/> or <see cref="OnAdvertisingFailed()"/> 
    /// will be called once we've found out if we successfully entered this mode. </remarks>
    private async Task StartAdvertising()
    {
        string localEndpointName = AdvertisingName;
        var options = new AdvertisingOptions.Builder().SetStrategy(this.Strategy);
        var advertising = this.ConnectionsClient!.StartAdvertisingAsync(localEndpointName, this.ServiceId,
            new ConnectionLifecycleCallback(this), options.Build());
        await advertising;

        if (advertising.IsCompletedSuccessfully)
        {
            Logger.Verbose($"Now advertising endpoint {localEndpointName}");
            this.OnAdvertisingStarted();
        }

        if (advertising.IsFaulted)
        {
            this.IsAdvertising = false;
            Logger.Warn($"startAdvertising() failed. {advertising.Exception}");
            this.OnAdvertisingFailed();
        }
    }

    /// <summary> Sets the device to discovery mode. It will now listen for devices in advertising mode.
    /// Either <see cref="OnDiscoveryStarted()"/> or <see cref="OnDiscoveryFailed()"/>
    /// will be called once we've found out if we successfully entered this mode. </summary>
    private async Task StartDiscovering()
    {
        this.DiscoveredEndpoints.Clear();
        var discoveryOptions = new DiscoveryOptions.Builder().SetStrategy(this.Strategy);
        var discovery = this.ConnectionsClient!.StartDiscoveryAsync(this.ServiceId, new EndpointDiscoveryCallback(this), discoveryOptions.Build()); await discovery;

        if (discovery.IsCompletedSuccessfully) this.OnDiscoveryStarted();
        if (discovery.IsFaulted)
        {
            this.IsDiscovering = false;
            Logger.Warn($"{this.StartDiscovering}() failed. {discovery.Exception}");
            this.OnDiscoveryFailed();
        }
    }

    private void StopDiscovering() => this.ConnectionsClient!.StopDiscovery();
    private void StopAdvertising() => this.ConnectionsClient!.StopAdvertising();

    /// <summary> Disconnects from the given endpoint </summary>
    protected void Disconnect(EndPoint endpoint)
    {
        this.ConnectionsClient!.DisconnectFromEndpoint(endpoint.Id);
        this.EstablishedConnections.Remove(endpoint.Id);
    }

    /// <summary> Disconnects from all currently connected endpoints </summary>
    protected void DisconnectFromAllEndpoints()
    {
        foreach (EndPoint endpoint in this.EstablishedConnections.Values) this.ConnectionsClient!.DisconnectFromEndpoint(endpoint.Id);
        this.EstablishedConnections.Clear();
    }

    /// <summary> Sends a connection request to the endpoint. Either <see cref="OnConnectionInitiated(EndPoint, ConnectionInfo)"/> or 
    /// <see cref="OnConnectionFailed(EndPoint)"/> will be called once we've found out if we successfully reached the device. </summary>
    protected async void ConnectToEndpoint(EndPoint endpoint)
    {
        Logger.Verbose($"Sending a connection request to endpoint {endpoint}");
        // Mark ourselves as connecting so we don't connect multiple times
        this.IsConnecting = true;

        // Ask to connect
        var connection = this.ConnectionsClient!.RequestConnectionAsync(this.AdvertisingName, endpoint.Id, new ConnectionLifecycleCallback(this));
        await connection;

        if (connection.IsFaulted)
        {
            Logger.Warn($"{nameof(ConnectionsClient.RequestConnection)} failed. {connection.Exception}");
            this.IsConnecting = false;
            this.OnConnectionFailed(endpoint);
        }
    }

    /// <summary>
    /// adds the endpoint to the dictionary of connected endpoints
    /// and calls the OnEndpointConnected callback function
    /// </summary>
    /// <param name="endpoint">the endpoint we successfully connected to</param>
    protected void ConnectedToEndpoint(EndPoint endpoint)
    {
        Logger.Debug($"{nameof(ConnectedToEndpoint)}({nameof(endpoint)}={endpoint})");
        this.EstablishedConnections.Add(endpoint.Id, endpoint);
        this.OnEndpointConnected(endpoint);
    }

    /// <summary>
    /// removes the endpoint from the dictionary of connected endpoints 
    /// and calls the OnEndpointDisconnected callback function
    /// </summary>
    /// <param name="endpoint">the endpoint we disconnected from</param>
    protected void DisconnectedFromEndpoint(EndPoint endpoint)
    {
        Logger.Debug($"{nameof(DisconnectedFromEndpoint)}({nameof(endpoint)}={endpoint})");
        this.EstablishedConnections.Remove(endpoint.Id);
        this.OnEndpointDisconnected(endpoint);
    }

    /// <summary> Resets and clears all state in Nearby Connections. </summary>
    protected void StopAllEndpoints(bool disconnect = false)
    {
        if (disconnect) this.DisconnectFromAllEndpoints();
        this.IsAdvertising = false;
        this.IsDiscovering = false;
        this.IsConnecting = false;
        this.ConnectionsClient!.StopAllEndpoints();
        this.DiscoveredEndpoints.Clear();
        this.PendingConnections.Clear();
        this.EstablishedConnections.Clear();
    }

    /// <summary> Accepts a connection request </summary>
    protected async Task AcceptConnection(EndPoint endpoint)
    {
        Task? accept = this.ConnectionsClient!.AcceptConnectionAsync(endpoint.Id, new PayloadCallback(this)); await accept;
        if (accept.IsFaulted) Logger.Warn($"{this.AcceptConnection}() failed. {accept.Exception}");
    }

    /// <summary> Rejects a connection request </summary>
    protected async void RejectConnection(EndPoint endpoint)
    {
        var reject = this.ConnectionsClient!.RejectConnectionAsync(endpoint.Id); await reject;
        if (reject.IsFaulted) Logger.Warn($"{this.RejectConnection}() failed. {reject.Exception}");
    }

    /// <summary> Sends a <see cref="Payload"/> to all currently connected endpoints. </summary>
    /// <param name="payload">The data you want to send. </param>
    public async virtual void Send(Payload payload) => await this.Send(payload, this.EstablishedConnections.Keys);

    /// <summary> Called when advertising successfully starts. Override this method to act on the event. </summary>
    protected virtual void OnAdvertisingStarted() => Logger.Verbose(nameof(OnAdvertisingStarted));

    /// <summary> Called when advertising fails to start. Override this method to act on the event. </summary>
    protected virtual void OnAdvertisingFailed() => Logger.Verbose(nameof(OnAdvertisingFailed));

    /// <summary> Called when discovery successfully starts. Override this method to act on the event. </summary>
    protected virtual void OnDiscoveryStarted() => Logger.Verbose(nameof(OnDiscoveryStarted));

    /// <summary> Called when discovery fails to start. Override this method to act on the event. </summary>
    protected virtual void OnDiscoveryFailed() => Logger.Verbose(nameof(OnDiscoveryFailed));

    /// <summary> 
    /// Called when a remote endpoint is discovered, not to be confused 
    /// with <see cref="ConnectToEndpoint(EndPoint)"/> which is called when connecting to the device. 
    /// </summary>
    protected virtual void OnEndpointDiscovered(EndPoint endpoint) => Logger.Verbose(nameof(OnEndpointDiscovered));

    /// <summary> Called when a pending connection with a remote endpoint is created.
    /// Use <see cref="ConnectionInfo"/> for metadata about the connection (like incoming vs outgoing, or the authentication token). 
    /// If we want to continue with the connection, call <see cref="AcceptConnection(EndPoint)"/>.
    /// Otherwise, call <see cref="RejectConnection(EndPoint)"/>. </summary>
    protected abstract void OnConnectionInitiated(EndPoint endpoint, ConnectionInfo connectionInfo);

    /// <summary> Called when a connection with this endpoint has failed. Override this method to act on the event. </summary>
    protected abstract void OnConnectionFailed(EndPoint endpoint);

    /// <summary> Called when someone has connected to us. Override this method to act on the event. </summary>
    protected abstract void OnEndpointConnected(EndPoint endpoint);

    /// <summary> Called when someone has disconnected. Override this method to act on the event. </summary>
    protected abstract void OnEndpointDisconnected(EndPoint endpoint);

    /// <remarks> Someone connected to us has sent us data. Override this method to act on the event. </remarks>
    /// <param name="endpoint">endpoint The sender </param>
    /// <param name="payload">payload The data </param>
    protected abstract void OnReceive(EndPoint endpoint, Payload payload);

    private async Task Send(Payload payload, Dictionary<string, EndPoint>.KeyCollection endpoints)
    {
        var sent = this.ConnectionsClient!.SendPayloadAsync([.. endpoints], payload); await sent;
        if (sent.IsFaulted) Logger.Warn($"{nameof(Send)}({nameof(Payload)} {nameof(payload)}) failed. {sent.Exception}");
    }

    private class PayloadCallback(ConnectionsActivity instance) : NearbyPayloadCallback()
    {
        public override void OnPayloadReceived(string endpointId, Payload payload)
        {
            Logger.Debug($"{nameof(OnPayloadReceived)}({nameof(endpointId)}={endpointId}, {nameof(payload)}={payload}");
            instance.OnReceive(instance.EstablishedConnections[endpointId], payload);
        }

        public override void OnPayloadTransferUpdate(string endpointId, PayloadTransferUpdate update) => Logger.Debug($"{nameof(OnPayloadTransferUpdate)}({nameof(endpointId)}={endpointId}, {nameof(update)}={update}");
    };

    private class EndpointDiscoveryCallback(ConnectionsActivity instance) : NearbyEndpointDiscoveryCallback()
    {
        public override void OnEndpointFound(string endpointId, DiscoveredEndpointInfo info)
        {
            Logger.Debug($"{nameof(OnEndpointFound)}({nameof(endpointId)}={endpointId}, {nameof(info.ServiceId)}={info.ServiceId}, {nameof(info.EndpointName)}={info.EndpointName})");
            if (instance.ServiceId.Equals(info.ServiceId))
            {
                var endpoint = new EndPoint(endpointId, info.EndpointName);
                Logger.Debug($"Endpoint created: {endpoint}");
                instance.DiscoveredEndpoints.Add(endpointId, endpoint);
                instance.OnEndpointDiscovered(endpoint);
            }
        }

        public override void OnEndpointLost(string endpointId) => Logger.Debug($"{nameof(OnEndpointLost)}({nameof(endpointId)}={endpointId})");
    }

    private class ConnectionLifecycleCallback(ConnectionsActivity instance) : NearbyConnectionLifecycleCallback()
    {
        public override void OnConnectionInitiated(string endpointId, ConnectionInfo connectionInfo)
        {
            Logger.Debug($"{nameof(OnConnectionInitiated)}({nameof(endpointId)}={endpointId}, {nameof(connectionInfo.EndpointName)}={connectionInfo.EndpointName})");

            var endPoint = new EndPoint(endpointId, connectionInfo.EndpointName);
            Logger.Debug($"Endpoint created: {endPoint}");
            instance.PendingConnections.Add(endpointId, endPoint);
            instance.OnConnectionInitiated(endPoint, connectionInfo);
        }

        /// <summary>
        /// removes the endpoint from the list of pending endpoints 
        /// when the result is successful adds the endpoint to the list of connected endpoints
        /// </summary>
        public override void OnConnectionResult(string endpointId, ConnectionResolution result)
        {
            Logger.Debug($"{nameof(OnConnectionResult)}({nameof(endpointId)}={endpointId}, {nameof(result)}={result})");
            instance.IsConnecting = false;
            if (!result.Status.IsSuccess)
            {
                Logger.Warn($"Connection failed. Received status {result.Status.Status()}");
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
                Logger.Warn($"Unexpected disconnection from endpoint {endpointId}");
                return;
            }

            instance.DisconnectedFromEndpoint(connection);
        }
    }
}
