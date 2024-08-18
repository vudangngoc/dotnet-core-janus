namespace DotnetCoreJanus;

using WebSocketSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using DotnetCoreJanus.Handler;

public class JanusClient
{
    public string Server { get; set; }
    public long Session { get; set; }

    private ConcurrentDictionary<string, IJanusHandler> handlers = new ConcurrentDictionary<string, IJanusHandler>();
    public ConcurrentDictionary<string, long> PluginHandleIds { get; set; } = new ConcurrentDictionary<string, long>();

    private WebSocket ws;

    private JsonSerializerOptions options = new JsonSerializerOptions 
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

    public void SendMessage(string transaction, string message, IJanusHandler handler)
    {
        Console.WriteLine("Sending to server: " + message);
        if(ws.ReadyState != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not open");
            return;
        }
        handlers.TryAdd(transaction, handler);
        ws.Send(message);
    }

    private void SendMessage(string message)
    {
        Console.WriteLine("Sending to server: " + message);
        if(ws.ReadyState != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not open");
            return;
        }
        ws.Send(message);
    }

    public void createSession()
    {
        var message = new
        {
            janus = "create",
            transaction = Guid.NewGuid().ToString()
        };
        handlers.TryAdd(message.transaction, new CreateSessionHandler(this));
        
        var json = JsonSerializer.Serialize(message, options);
        SendMessage(json);
    }

    public void AttacthPlugin(string plugin)
    {
        
        var message = new
        {
            janus = "attach",
            plugin = plugin,
            transaction = Guid.NewGuid().ToString(),
            session_id = this.Session
        };
        var json = JsonSerializer.Serialize(message, options);
        handlers.TryAdd(message.transaction, new AttachPluginHandler(this, plugin));
        SendMessage(json);
    }
}
