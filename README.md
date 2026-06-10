# SimpleServe.API (Docker)

## Docker Compose (recommended)
Run from the project root to build and start the API (detached):
```powershell
docker compose up --build --detach
```

View logs in the foreground:
```powershell
docker compose up --build
```

Stop and remove containers:
```powershell
docker compose down
```

Compose file (located at [docker-compose.yml](docker-compose.yml)):
```yaml
services:
  simpleserve-api:
    build:
      context: .
      dockerfile: SimpleServe.API/Dockerfile
    environment:
      ASPNETCORE_HTTP_PORTS: "8100"
      ASPNETCORE_ENVIRONMENT: "Development"
      Cleanup__IntervalSeconds: "1"
      Cleanup__HeartbeatTimeoutSeconds: "10"
    ports:
      - "8100:8100"
    restart: unless-stopped
```

Config env var:
- `Cleanup__IntervalSeconds` (default: `1`)
- `Cleanup__HeartbeatTimeoutSeconds` (default: `10`)

API base URL:
- `http://localhost:8100`

OpenAPI/Scalar (enabled by compose env):
- OpenAPI: `http://localhost:8100/openapi/v1.json`
- Scalar: `http://localhost:8100/scalar/v1`

## Build + Run
```powershell
docker build -t simpleserve-api -f SimpleServe.API/Dockerfile .
docker run --rm -p 8100:8100 -e ASPNETCORE_HTTP_PORTS=8100 -e Cleanup__IntervalSeconds=1 -e Cleanup__HeartbeatTimeoutSeconds=10 simpleserve-api
```

API base URL:
- `http://localhost:8100`

Optional OpenAPI/Scalar (Development only):
```powershell
docker run --rm -p 8100:8100 -e ASPNETCORE_HTTP_PORTS=8100 -e ASPNETCORE_ENVIRONMENT=Development -e Cleanup__IntervalSeconds=1 -e Cleanup__HeartbeatTimeoutSeconds=10 simpleserve-api
```
- OpenAPI: `http://localhost:8100/openapi/v1.json`
- Scalar: `http://localhost:8100/scalar/v1`

## Endpoints

### `POST /health`
Registers/refreshes a server heartbeat.

Request:
```json
{
  "id": "server-1",
  "metadata": {
	"map": "Dust2",
	"players": 12,
	"region": "eu-central"
  }
}
```

Response `200`:
```json
{
  "status": "Healthy",
  "serverId": "server-1"
}
```

### `GET /servers`
Returns all registered servers.

### `GET /servers/{id}`
Returns a single server or `404`.

### `DELETE /servers/{id}`
Removes a server. Returns `204` or `404`.

## Quick cURL
```powershell
curl -X POST http://localhost:8100/health -H "Content-Type: application/json" -d '{"id":"server-1","metadata":{"map":"Dust2","players":12}}'
curl http://localhost:8100/servers
curl http://localhost:8100/servers/server-1
curl -X DELETE http://localhost:8100/servers/server-1
```