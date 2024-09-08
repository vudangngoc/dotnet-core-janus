using System.Text.Json;
using System.Text.Json.Nodes;
using DotnetCoreJanus.TextRoom;
namespace DotnetCoreJanus
{
    internal class GetRoomsHandler : IJanusHandler
    {
        public GetRoomsHandler(TaskCompletionSource<List<RoomInfo>> result)
        {
            Result = result;
        }

        public TaskCompletionSource<List<RoomInfo>> Result { get; }

        public bool HandleMessage(JsonDocument doc)
        {
            if(doc.RootElement.TryGetProperty("janus", out JsonElement janus) && janus.GetString() == "success")
            {
                if(doc.RootElement.TryGetProperty("plugindata", out JsonElement plugindata))
                {
                    if(plugindata.TryGetProperty("data", out JsonElement data) )
                    {
                        if(data.TryGetProperty("textroom", out JsonElement textroom) && textroom.GetString() == "success")
                        {
                            data.TryGetProperty("list", out JsonElement rooms);
                            List<RoomInfo> roomInfos = [];
                            foreach (JsonElement jsonElement in rooms.EnumerateArray())
                            {
                                RoomInfo roomInfo = new RoomInfo
                                {
                                    Room = jsonElement.GetProperty("room").GetInt64(),
                                    Description = jsonElement.GetProperty("description").GetString(),
                                    PinRequired = jsonElement.GetProperty("pin_required").GetBoolean(),
                                    History = jsonElement.GetProperty("history").GetInt64(),
                                    NumberParticipants = jsonElement.GetProperty("num_participants").GetInt64()
                                };
                                roomInfos.Add(roomInfo);
                            }
                            Result.SetResult(roomInfos);
                            return true;
                        }
                    }
                }
            }
            Result.SetResult([]);
            return true;
        }
    }
}