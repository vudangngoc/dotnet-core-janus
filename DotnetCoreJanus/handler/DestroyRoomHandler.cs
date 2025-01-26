using System.Text.Json;
using System.Text.Json.Nodes;

namespace DotnetCoreJanus.handler
{
    internal class DestroyRoomHandler : IJanusHandler
    {
        public DestroyRoomHandler(TaskCompletionSource<JsonObject> result)
        {
            Result = result;
        }

        public TaskCompletionSource<JsonObject> Result { get; }

        public bool HandleMessage(JsonDocument doc)
        {
            if (doc.RootElement.TryGetProperty("janus", out var janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("plugindata", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("data", out JsonElement data))
                    {
                        Result.SetResult(JsonObject.Create(data));
                    }
                }
                return true;
            }

            return false;
        }
    }
}