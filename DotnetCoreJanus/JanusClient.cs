namespace DotnetCoreJanus;

using WebSocketSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using DotnetCoreJanus.Handler;
using System.Text.Json.Nodes;
using Microsoft.MixedReality.WebRTC;

public class JanusClient
{
    public string Server { get; set; }

    private readonly ConcurrentDictionary<string, IJanusHandler> handlers = new();
    private readonly ConcurrentDictionary<string, long> _sessions = new(); 
    private readonly WebSocket ws;

    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public JanusClient(string server)
    {
        this.Server = server;
        this.ws = new WebSocket(server, new string[] { "janus-protocol" });
        
        ws.OnMessage += (sender, e) =>
                    OnMessage(sender, e);

        ws.OnOpen += (sender, e) =>
        {
            Console.WriteLine("WebSocket Open");
        };

        // Connect to the WebSocket server
        ws.Connect();

    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        Console.WriteLine("Received from server: " + e.Data);
        JsonDocument doc = JsonDocument.Parse(e.Data);
        string transaction = doc.RootElement.GetProperty("transaction").GetString();
        IJanusHandler? handler = handlers.TryGetValue(transaction, out handler) ? handler : null;
        if(handler != null && handler.HandleMessage(doc))
        {
            handlers.TryRemove(transaction, out _);
        }
        
    }

    public void Close()
    {
        ws.Close();
    }

    public void SendMessage(string userName, JsonObject doc, IJanusHandler handler)
    {
        
        if(ws.ReadyState != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not open");
            return;
        }
        this._sessions.TryGetValue(userName, out long session_id);
        doc.Add("session_id", session_id);
        string transaction = Guid.NewGuid().ToString();
        doc.Add("transaction", transaction);
        string message = doc.ToJsonString();
        
        this.handlers.TryAdd(transaction, handler);
        Console.WriteLine("Sending to server: " + message);
        ws.Send(message);
    }

    private void SendMessage(string message)
    {
        
        if(ws.ReadyState != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not open");
            return;
        }
        Console.WriteLine("Sending to server: " + message);
        ws.Send(message);
    }

    public void CreateSession(string userName)
    {
        var message = new
        {
            janus = "create",
            transaction = Guid.NewGuid().ToString()
        };
        var response = new TaskCompletionSource<long>();
        handlers.TryAdd(message.transaction, new CreateSessionHandler(response));
        
        var json = JsonSerializer.Serialize(message, options);
        SendMessage(json);
        this._sessions.TryAdd(userName, response.Task.Result);
    }

    public long AttacthPlugin(string userName, string plugin)
    {
        this._sessions.TryGetValue(userName, out long session_id);
        var message = new
        {
            janus = "attach",
            plugin = plugin,
            transaction = Guid.NewGuid().ToString(),
            session_id = session_id
        };
        var json = JsonSerializer.Serialize(message, options);
        var response = new TaskCompletionSource<long>();
        handlers.TryAdd(message.transaction, new AttachPluginHandler(plugin, response));
        SendMessage(json);
        return response.Task.Result;
    }

    public void Disconnect(string userName)
    {
        this._sessions.TryRemove(userName, out long _);
    }

    internal PeerConnection handleSdpOffer(string userName, long handle_id, string sdoOffer)
    {
        SdpMessage remoteSdp = new SdpMessage
            {
                Content = sdoOffer,
                Type = SdpMessageType.Offer // or Answer depending on your role
            };
        var peerConnection = CreatePeerConnection();
        peerConnection.SetRemoteDescriptionAsync(remoteSdp);
        if( peerConnection.CreateAnswer()){
            // var answer = await peerConnection.
        }
        return peerConnection;
    }


    private PeerConnection CreatePeerConnection()
        {
            var peerConnection = new PeerConnection();

            // Initialize the PeerConnection with a configuration (optional)
            var config = new PeerConnectionConfiguration
            {
                IceServers = new List<IceServer>
                {
                    new IceServer { Urls = new List<string> { "stun:stun.l.google.com:19302" } }
                }
            };

            // Initialize the PeerConnection (this prepares the WebRTC stack)
            peerConnection.InitializeAsync(config);

            // Handle the connection state changes (optional)
            peerConnection.Connected += () =>
            {
                Console.WriteLine("Peer connection established!");
            };

            // Handle ICE candidates being gathered (optional)
            peerConnection.IceCandidateReadytoSend += (candidate) =>
            {
                Console.WriteLine($"New ICE candidate: {candidate}");
            };

            peerConnection.IceStateChanged += (state) =>
            {
                Console.WriteLine($"ICE state: {state}");
            };

            return peerConnection;
        }
}
