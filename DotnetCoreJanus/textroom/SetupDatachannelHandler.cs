using System.Text.Json;

namespace DotnetCoreJanus
{
    internal class SetupDatachannelHandler : IJanusHandler
    {
        private TaskCompletionSource<string> response;

        public SetupDatachannelHandler(TaskCompletionSource<string> response)
        {
            this.response = response;
        }

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement ack) && ack.GetString() == "ack")
            {
                return false;
            } 
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "event")
            {
                if(doc.RootElement.TryGetProperty("jsep", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("sdp", out JsonElement sdp))
                    {
                        response.SetResult(sdp.GetString());
                        return true;
                    }
                }
            } else if(doc.RootElement.TryGetProperty("janus", out janus) && janus.GetString() == "error")
            {
                response.SetException(new System.Exception("Error setup datachannel: " + doc.RootElement.GetProperty("error").GetString()));
            }
            return true;
            
        }
    }
}