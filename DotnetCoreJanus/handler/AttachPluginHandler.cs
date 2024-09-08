using System;
using System.Text.Json;

namespace DotnetCoreJanus.Handler
{
    public class AttachPluginHandler : IJanusHandler
    {
        private  TaskCompletionSource<long> result;
        public AttachPluginHandler(TaskCompletionSource<long> result)
        {
            this.result = result;
        }
        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("data", out JsonElement data))
                {
                    this.result.SetResult(data.GetProperty("id").GetInt64());
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error attach to plugin: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}