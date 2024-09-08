using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DotnetCoreJanus.TextRoom;
namespace DotnetCoreJanus
{
    public class TextRoomPlugin
    {
        private readonly JanusClient _janusClient;

        public TextRoomPlugin(JanusClient janusClient)
        {
            _janusClient = janusClient ?? throw new ArgumentNullException(nameof(janusClient));
        }

        public List<RoomInfo> GetRooms(long sessionId, long handleId)
        {
            string transaction = Guid.NewGuid().ToString();
            var message = new JsonObject
            {
                ["janus"] = "message",
                ["transaction"] = transaction,
                ["handle_id"] = handleId,
                ["session_id"] = sessionId
            };
            var body = new JsonObject
            {
                ["request"] = "list"
            };
            message["body"] = body;
            
            TaskCompletionSource<List<RoomInfo>> result =  new();
            _janusClient.SendMessage(transaction, message, new GetRoomsHandler(result));
            var output = result.Task.Result;
            return output;
        }

        public string SetupPeerConnection(long sessionId, long handleId)
        {
            string transaction = Guid.NewGuid().ToString();
            var message = new JsonObject
            {
                ["janus"] = "message",
                ["transaction"] = transaction,
                ["handle_id"] = handleId,
                ["session_id"] = sessionId
            };
            var body = new JsonObject
            {
                ["request"] = "setup"
            };
            message["body"] = body;
            
            TaskCompletionSource<string> result =  new();
            _janusClient.SendMessage(transaction, message, new SetupPeerConnectionHandler(result));
            var output = result.Task.Result;
            return output;
        }

        public string SendSdpAnswer(long sessionId, long handleId, string answer)
        {
            string transaction = Guid.NewGuid().ToString();
            var message = new JsonObject
            {
                ["janus"] = "message",
                ["transaction"] = transaction,
                ["handle_id"] = handleId,
                ["session_id"] = sessionId,
            };
            var body = new JsonObject
            {
                ["request"] = "ack"
            };
            message["body"] = body;
            var jsep = new JsonObject
            {
                ["type"] = "answer",
                ["sdp"] = answer
            };
            message["jsep"] = jsep;
            TaskCompletionSource<string> result =  new();
            _janusClient.SendMessage(transaction, message, new SendSdpAnswernHandler(result));
            var output = result.Task.Result;
            return output;
        }
    }
}