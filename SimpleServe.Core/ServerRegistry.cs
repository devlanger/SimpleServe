using System.Collections.Concurrent;

namespace SimpleServe.Core;

public class ServerRegistry
{
    public ConcurrentDictionary<string, GameServer> Servers { get; } = new();
}
