using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DotnetCoreJanus.handler;
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

        public long CreateRoom(long sessionId, long handleId, string roomName)
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
                ["request"] = "create",
                ["description"] = roomName
            };
            message["body"] = body;
            TaskCompletionSource<long> result = new();
            _janusClient.SendMessage(transaction, message, new CreateRoomHandler(result));
            return result.Task.Result;
        }

        public JsonObject DestroyRoom(long sessionId, long handleId, long roomId)
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
                ["request"] = "destroy",
                ["room"] = roomId
            };
            message["body"] = body;
            TaskCompletionSource<JsonObject> result = new();
            _janusClient.SendMessage(transaction, message, new DestroyRoomHandler(result));
            return result.Task.Result;
        }

        public JsonArray GetParticipants(long sessionId, long handleId, long roomId)
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
                ["request"] = "listparticipants",
                ["room"] = roomId
            };
            message["body"] = body;
            TaskCompletionSource<JsonArray> result = new();
            _janusClient.SendMessage(transaction, message, new GetParticipantsHandler(result));
            return result.Task.Result;
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

    }
}