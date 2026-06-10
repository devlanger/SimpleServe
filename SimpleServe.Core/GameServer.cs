using System.Collections.Concurrent;
using System.Text.Json;

namespace SimpleServe.Core;

public sealed class GameServer
{
    public required string Id { get; init; }

    public DateTime LastHeartbeat { get; set; }

    public ConcurrentDictionary<string, JsonElement> Metadata { get; } = new();
}
