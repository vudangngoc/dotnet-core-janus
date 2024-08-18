using DotnetCoreJanus;

namespace DotnetCoreJanus.Tests;

public class TestJanusClient
{
    [Fact]
    public void TestAttachPlugin()
    {
        JanusClient janusClient = new JanusClient("wss://janus.conf.meetecho.com/ws");
        Thread.Sleep(1000);
        janusClient.createSession();
        Thread.Sleep(1000);
        janusClient.AttacthPlugin("janus.plugin.textroom");
        Thread.Sleep(1000);
        Assert.True(janusClient.PluginHandleIds.TryGetValue("janus.plugin.textroom", out long handleId));
    }
}