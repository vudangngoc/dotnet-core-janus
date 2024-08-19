using System.Text.Json.Nodes;
using DotnetCoreJanus;

namespace DotnetCoreJanus.Tests;

public class TestJanusClient
{
    [Fact]
    public void TestAttachPlugin()
    {
        JanusClient janusClient = new JanusClient("ws://125.212.229.11:8188/ws");
        string userName = "abc";
        janusClient.CreateSession(userName);
        long handle_id = janusClient.AttacthPlugin(userName, "janus.plugin.textroom");
        // Thread.Sleep(1000);
        Assert.True(handle_id > 0);
        TextRoomPlugin textRoomPlugin = new TextRoomPlugin(janusClient);
        textRoomPlugin.HandleIds.TryAdd(userName,handle_id);
        Assert.True(textRoomPlugin.getAllRooms(userName).Count > 0);
        var offer = textRoomPlugin.SetupDatachannel(userName);
    }
}