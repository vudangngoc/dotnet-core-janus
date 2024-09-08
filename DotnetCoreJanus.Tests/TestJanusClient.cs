using DotnetCoreJanus;

namespace DotnetCoreJanus.Tests;

public class TestJanusClient
{
    [Fact]
    public void TestAttachPlugin()
    {
        JanusClient janusClient = new JanusClient("ws://159.65.129.9:8188/ws");
        Thread.Sleep(1000);
        long sessionId = janusClient.CreateSession();
        long handleId = janusClient.AttacthPlugin(sessionId, "janus.plugin.textroom");
        TextRoomPlugin textRoomPlugin = new TextRoomPlugin(janusClient);
        Assert.True(handleId > 0);
        var rooms = textRoomPlugin.GetRooms(sessionId, handleId);
        Assert.True(rooms.Count > 0);
        string sdpOffer = textRoomPlugin.SetupPeerConnection(sessionId, handleId);
        Assert.False(sdpOffer.Equals(""));
        string sendAnswerOutput = textRoomPlugin.SendSdpAnswer(sessionId, handleId, "");
        Assert.False(sendAnswerOutput.Equals("error"));
    }
}