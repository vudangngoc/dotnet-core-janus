using System;
using System.Text.Json;

namespace DotnetCoreJanus.Handler
{
    public class CreateSessionHandler(TaskCompletionSource<long> result) : IJanusHandler
    {
        private TaskCompletionSource<long> result = result;

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("data", out JsonElement data))
                {
                    result.SetResult(data.GetProperty("id").GetInt64());
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error creating session: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}