using System.Text.Json;

namespace SimpleServe.Core;

public sealed record HealthRequest(
    string Id,
    Dictionary<string, JsonElement>? Metadata);
