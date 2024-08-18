using System.Text.Json;

public interface IJanusHandler
{
    bool HandleMessage(JsonDocument doc);
}