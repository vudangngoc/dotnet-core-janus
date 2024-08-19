using System.Text.Json;
using System.Text.Json.Nodes;

namespace DotnetCoreJanus
{
    internal class ListRoomsHandler : IJanusHandler
    {
        private TaskCompletionSource<JsonArray> response;

        public ListRoomsHandler(TaskCompletionSource<JsonArray> response)
        {
            this.response = response;
        }

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("plugindata", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("data", out JsonElement data))
                    {
                        JsonArray rooms = new JsonArray();
                        foreach(JsonElement room in data.GetProperty("list").EnumerateArray())
                        {
                            rooms.Add(room);
                        }
                        response.SetResult(rooms);
                        return true;
                    }
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                response.SetException(new System.Exception("Error getting list of rooms: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString()));
            }
            return true;
        }
    }
}