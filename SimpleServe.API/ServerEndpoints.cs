using SimpleServe.Core;

namespace SimpleServe.API;

public static class ServerEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/health", (
            HealthRequest request,
            ServerRegistry registry) =>
        {
            var server = registry.Servers.GetOrAdd(
                request.Id,
                id => new GameServer
                {
                    Id = id
                });

            server.LastHeartbeat = DateTime.UtcNow;

            if (request.Metadata is not null)
            {
                foreach (var (key, value) in request.Metadata)
                {
                    server.Metadata[key] = value;
                }
            }

            return Results.Ok(new HealthResponse("Healthy", server.Id));
        })
        .Accepts<HealthRequest>("application/json")
        .Produces<HealthResponse>(StatusCodes.Status200OK)
        .WithSummary("Registers or refreshes a server heartbeat")
        .WithDescription("Request example: { \"id\": \"server-1\", \"metadata\": { \"map\": \"Dust2\", \"players\": 12 } } Response example: { \"status\": \"Healthy\", \"serverId\": \"server-1\" }");

        app.MapGet("/servers", (ServerRegistry registry) =>
        {
            return Results.Ok(registry.Servers.Values);
        })
        .Produces<IEnumerable<GameServer>>(StatusCodes.Status200OK);

        app.MapGet("/servers/{id}", (
            string id,
            ServerRegistry registry) =>
        {
            return registry.Servers.TryGetValue(id, out var server)
                ? Results.Ok(server)
                : Results.NotFound();
        })
        .Produces<GameServer>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/servers/{id}", (
            string id,
            ServerRegistry registry) =>
        {
            return registry.Servers.TryRemove(id, out _)
                ? Results.NoContent()
                : Results.NotFound();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}

public sealed record HealthResponse(
    string Status,
    string ServerId);
