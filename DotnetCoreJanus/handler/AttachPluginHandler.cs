using System;
using System.Text.Json;

namespace DotnetCoreJanus.Handler
{
    public class AttachPluginHandler : IJanusHandler
    {
        private JanusClient JanusClient;
        private string pluginName;
        public AttachPluginHandler(JanusClient janusClient, string pluginName)
        {
            this.pluginName = pluginName;
            this.JanusClient = janusClient;
        }
        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("data", out JsonElement data))
                {
                    JanusClient.PluginHandleIds.TryAdd(this.pluginName, data.GetProperty("id").GetInt64());
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error attach to plugin " + this.pluginName +": " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}