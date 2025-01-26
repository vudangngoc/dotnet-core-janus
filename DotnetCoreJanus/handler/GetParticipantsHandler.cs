using System.Text.Json;
using System.Text.Json.Nodes;
namespace DotnetCoreJanus
{
    internal class GetParticipantsHandler : IJanusHandler
    {
        public GetParticipantsHandler(TaskCompletionSource<JsonArray> result)
        {
            Result = result;
        }

        public TaskCompletionSource<JsonArray> Result { get; }

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("plugindata", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("data", out JsonElement data))
                    {
                        if (data.TryGetProperty("participants", out JsonElement participants) )
                        {
                            Result.SetResult(JsonArray.Create(participants.Clone()));
                        }
                        else
                        {
                            Result.SetResult(null);
                        }
                    } else
                    {
                        Result.SetResult(null);
                    }
                } else
                {
                    Result.SetResult(null);
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                Console.WriteLine("Error get participants: " + doc.RootElement.GetProperty("error").GetProperty("reason").GetString());
                Result.SetResult(null);
            }
            
            return true;
        }
    }
}