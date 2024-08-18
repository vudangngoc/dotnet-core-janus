using System;
using System.Text.Json;

namespace DotnetCoreJanus.Handler
{
    public class CreateSessionHandler : IJanusHandler
    {
        private JanusClient JanusClient;
        public CreateSessionHandler(JanusClient janusClient)
        {
            this.JanusClient = janusClient;
        }
        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("data", out JsonElement data))
                {
                    JanusClient.Session = data.GetProperty("id").GetInt64();
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error creating session: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}