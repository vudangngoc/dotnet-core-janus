using System;
using System.Threading.Tasks;

namespace DotnetCoreJanus
{
    public class TextRoomPlugin
    {
        private readonly JanusClient _janusClient;

        public TextRoomPlugin(JanusClient janusClient)
        {
            _janusClient = janusClient ?? throw new ArgumentNullException(nameof(janusClient));
        }

        public async Task JoinRoom(long roomId, string displayName)
        {
            // try
            // {
            //      _janusClient.AttacthPlugin("janus.plugin.textroom");
            //     var joinRoomRequest = new
            //     {
            //         request = "join",
            //         room = roomId,
            //         display = displayName
            //     };

            //     await _janusClient.SendMessage(pluginHandle, joinRoomRequest);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Failed to join the room: {ex.Message}");
            // }
        }

        public async Task SendMessage(string message)
        {
            
        }

        public async Task LeaveRoom()
        {
            
        }
    }
}