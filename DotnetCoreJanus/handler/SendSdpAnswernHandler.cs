using System.Text.Json;
namespace DotnetCoreJanus
{
    internal class SendSdpAnswernHandler : IJanusHandler
    {
        public SendSdpAnswernHandler(TaskCompletionSource<string> result)
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
            if(janus.Equals("error")){
                if(doc.RootElement.TryGetProperty("error", out JsonElement error))
                {
                    if(error.TryGetProperty("reason", out JsonElement reason))
                    {
                        Result.SetResult(reason.GetString());
                        return true;
                    }
                }
                Result.SetResult("error");
                return true;
            }
            throw new NotImplementedException();
        }
    }
}