using System;
using System.Text.Json;

namespace DotnetCoreJanus.Handler
{
    public class AttachPluginHandler : IJanusHandler
    {
        private string pluginName;
        private TaskCompletionSource<long> response;
        public AttachPluginHandler(string pluginName, TaskCompletionSource<long> response)
        {
            this.pluginName = pluginName;
            this.response = response;
        }
        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("data", out JsonElement data))
                {
                    response.SetResult( data.GetProperty("id").GetInt64());
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error attach to plugin " + this.pluginName +": " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}