# FroniusMonitor.Gateway

## Purpose
Gateway API for browser-compatible Fronius live-data access.

## Endpoints
- `GET /health` - basic liveness.
- `GET /api/config` - current gateway endpoint configuration.
- `GET /api/snapshot` - single live snapshot.
- `GET /api/snapshot/live?count=5&intervalMs=1000` - short live sequence.

## Browser integration
The browser host should call the gateway API instead of direct Fronius host requests.

## Startup order
1. Start `FroniusMonitor.Gateway`.
2. Verify `GET /health` returns `Ok`.
3. Start `FroniusMonitor.Avalonia.Browser`.
4. Ensure browser host can reach gateway URL (CORS enabled by default).
