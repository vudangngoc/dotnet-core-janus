namespace DotnetCoreJanus;

using WebSocketSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using DotnetCoreJanus.Handler;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

public class JanusClient
{

    private readonly ILogger<JanusClient> _logger;
    public string Server { get; set; }

    private ConcurrentDictionary<string, IJanusHandler> handlers = new ConcurrentDictionary<string, IJanusHandler>();

    private WebSocket ws;

    private JsonSerializerOptions options = new JsonSerializerOptions 
    {
        WriteIndented = true
    };

    public JanusClient(string server, ILogger<JanusClient> logger)
    {
        _logger = logger;
        this.Server = server;
        this.ws = new WebSocket(server, new string[] { "janus-protocol" });
        
        ws.OnMessage += (sender, e) =>
                    OnMessage(sender, e);

        ws.OnOpen += (sender, e) =>
        {
            _logger.LogInformation("WebSocket Open");
        };

        // Connect to the WebSocket server
        ws.Connect();

    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        _logger.LogInformation("Received from server: " + e.Data);
        JsonDocument doc = JsonDocument.Parse(e.Data);
        if (!doc.RootElement.TryGetProperty("transaction", out JsonElement transactionElement))
        {
            return;
        }
        string transaction = transactionElement.GetString();
        IJanusHandler? handler = handlers.TryGetValue(transaction, out handler) ? handler : null;
        if (handler != null && handler.HandleMessage(doc))
        {
            handlers.TryRemove(transaction, out _);
        }
    }

    public void Close()
    {
        ws.Close();
    }

    public void SendMessage(string transaction, JsonObject message, IJanusHandler handler)
    {
        _logger.LogInformation("Sending to server: " + message);
        if(ws.ReadyState != WebSocketState.Open)
        {
            _logger.LogError("WebSocket is not open");
            return;
        }
        handlers.TryAdd(transaction, handler);
        ws.Send(message.ToString());
    }

    private void SendMessage(string message)
    {
        _logger.LogInformation("Sending to server: " + message);
        if(ws.ReadyState != WebSocketState.Open)
        {
            _logger.LogError("WebSocket is not open");
            return;
        }
        ws.Send(message);
    }

    public long CreateSession()
    {
        var message = new
        {
            janus = "create",
            transaction = Guid.NewGuid().ToString()
        };
        TaskCompletionSource<long> result = new TaskCompletionSource<long>();
        handlers.TryAdd(message.transaction, new CreateSessionHandler(result));
        
        var json = JsonSerializer.Serialize(message, options);
        SendMessage(json);
        return result.Task.Result;
    }

    public long AttacthPlugin(long sessionId, string plugin)
    {
        var message = new
        {
            janus = "attach",
            plugin = plugin,
            transaction = Guid.NewGuid().ToString(),
            session_id = sessionId
        };
        var json = JsonSerializer.Serialize(message, options);
        TaskCompletionSource<long> result = new();
        handlers.TryAdd(message.transaction, new AttachPluginHandler(result));
        SendMessage(json);
        return result.Task.Result;
    }
}
