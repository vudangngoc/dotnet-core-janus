using System.Text.Json;
using System.Text.Json.Nodes;

namespace DotnetCoreJanus
{
    internal class JoinRoomHandler : IJanusHandler
    {
        private TaskCompletionSource<JsonArray> response;

        public JoinRoomHandler(TaskCompletionSource<JsonArray> response)
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
                        if(data.TryGetProperty("participants", out JsonElement participants))
                        {
                            JsonArray participantsArray = new JsonArray();
                            foreach(JsonElement participant in participants.EnumerateArray())
                            {
                                participantsArray.Add(participant);
                            }
                            response.SetResult(participantsArray);
                            return true;
                        }
                        if(data.TryGetProperty("error", out JsonElement error))
                        {
                            response.SetException(new System.Exception("Error join room: " + error.GetString()));
                            return true;
                        }
                    }
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                response.SetException(new System.Exception("Error join room: " + doc.RootElement.GetProperty("error").GetString()));
            }
            return true;
        }
    }
}