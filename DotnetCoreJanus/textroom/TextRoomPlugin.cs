using System;
using System.Collections.Concurrent;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;

namespace DotnetCoreJanus
{
    public class TextRoomPlugin
    {
        private readonly JanusClient _janusClient;
        public ConcurrentDictionary<string, long> HandleIds = new();

        public TextRoomPlugin(JanusClient janusClient)
        {
            _janusClient = janusClient ?? throw new ArgumentNullException(nameof(janusClient));
        }

        public PeerConnection SetupDatachannel(string userName){
            this.HandleIds.TryGetValue(userName, out long handle_id);
            JsonObject setupDatachannelRequest = new JsonObject
            {
                ["janus"] = "message",
                ["handle_id"] = handle_id,
                ["body"] = new JsonObject
                {
                    ["request"] = "setup"
                }
            };
            var response = new TaskCompletionSource<string>();
            _janusClient.SendMessage(userName, setupDatachannelRequest, new SetupDatachannelHandler(response));

            var result = _janusClient.handleSdpOffer(userName, handle_id, response.Task.Result);

            return result;
        }

        
        public List<Participant> JoinRoom(string userName, long roomId, string displayName)
        {
            this.HandleIds.TryGetValue(userName, out long handle_id);
            JsonObject joinRoomRequest = new JsonObject
            {
                ["janus"] = "message",
                ["handle_id"] = handle_id,
                ["body"] = new JsonObject
                {
                    ["textroom"] = "join",
                    ["room"] = roomId,
                    ["username"] = userName,
                    ["display"] = displayName
                }
            };
            var response = new TaskCompletionSource<JsonArray>();
            _janusClient.SendMessage(userName, joinRoomRequest, new JoinRoomHandler(response));
            var output = response.Task.Result;
            List<Participant> participants = new List<Participant>();
            foreach(JsonNode participant in output)
            {
                participants.Add(JsonSerializer.Deserialize<Participant>(participant));
            }
            return participants;
        }

        public List<TextRoom> getAllRooms(string userName)
        {
            this.HandleIds.TryGetValue(userName, out long handle_id);
            JsonObject joinRoomRequest = new JsonObject
                {
                    ["janus"] = "message",
                    ["handle_id"] = handle_id,
                    ["body"] = new JsonObject
                    {
                        ["request"] = "list"
                    }
                    
                };
            var response = new TaskCompletionSource<JsonArray>();
            
            _janusClient.SendMessage(userName, joinRoomRequest, new ListRoomsHandler(response));
            var output = response.Task.Result;

            List<TextRoom> rooms = new List<TextRoom>();
            foreach(JsonNode room in output)
            {
                rooms.Add(JsonSerializer.Deserialize< TextRoom>(room));
            }
            return rooms;
        } 

        public async Task SendMessage(string userName, long roomId, string message)
        {
            
        }

        public async Task LeaveRoom()
        {
            
        }
    }
}