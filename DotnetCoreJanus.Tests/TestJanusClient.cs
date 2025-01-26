using System.Text.Json.Nodes;
using DotnetCoreJanus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
namespace DotnetCoreJanus.Tests;

public class TestJanusClient
{
    [Fact]
    public void TestAttachPlugin()
    {
        var serviceProvider = new ServiceCollection()
                                .AddLogging(configure => configure.AddConsole())
                                .AddTransient<JanusClient>()
                                .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILogger<JanusClient>>();
        JanusClient janusClient = new JanusClient("ws://143.198.212.46:8188/ws", logger);
        Thread.Sleep(1000);
        long sessionId = janusClient.CreateSession();
        long handleId = janusClient.AttacthPlugin(sessionId, "janus.plugin.textroom");
        TextRoomPlugin textRoomPlugin = new TextRoomPlugin(janusClient);
        Assert.True(handleId > 0);
        var rooms = textRoomPlugin.GetRooms(sessionId, handleId);
        Assert.True(rooms.Count > 0);
        long roomId = textRoomPlugin.CreateRoom(sessionId, handleId, "test room");
        Assert.True(roomId > 0);
        JsonArray participant = textRoomPlugin.GetParticipants(sessionId, handleId, roomId);
        Assert.NotNull(participant);
        Assert.True(participant.Count == 0);
        JsonObject destroyed = textRoomPlugin.DestroyRoom(sessionId, handleId, roomId);
        Assert.NotNull(destroyed);

    }
}