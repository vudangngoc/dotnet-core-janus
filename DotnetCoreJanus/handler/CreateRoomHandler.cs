using System.Text.Json;

namespace DotnetCoreJanus
{
    internal class CreateRoomHandler : IJanusHandler
    {
        public CreateRoomHandler(TaskCompletionSource<long> result)
        {
            Result = result;
        }

        public TaskCompletionSource<long> Result { get; }

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("plugindata", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("data", out JsonElement data))
                    {
                        Result.SetResult(data.GetProperty("room").GetInt64());
                    }
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error create room: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
            }
            return true;
        }
    }
}