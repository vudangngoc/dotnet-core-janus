using System.Text.Json;

namespace DotnetCoreJanus
{
    internal class SetupPeerConnectionHandler : IJanusHandler
    {
        public SetupPeerConnectionHandler(TaskCompletionSource<string> result)
        {
            Result = result;
        }

        public TaskCompletionSource<string> Result { get; }

        public bool HandleMessage(JsonDocument doc)
        {
            var janus = doc.RootElement.GetProperty("janus").GetString();
            if(janus == null)
            {
                throw new Exception();
            }
            if(janus.Equals("ack")){
                return false;
            }
            if(janus.Equals("event"))
            {
                if(doc.RootElement.TryGetProperty("jsep", out JsonElement jsep))
                {
                    Result.SetResult(jsep.GetProperty("sdp").GetString());
                    return true;
                }
            }
            
            Result.SetResult("");
            return true;
        }
    }
}