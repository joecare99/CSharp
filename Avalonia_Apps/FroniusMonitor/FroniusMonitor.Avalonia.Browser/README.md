# FroniusMonitor.Avalonia.Browser

## Runtime configuration
The browser host uses gateway-based live-data access.

Default gateway base URL in this slice:
- `http://localhost:5117/`

## Startup sequence
1. Start `FroniusMonitor.Gateway`.
2. Start `FroniusMonitor.Avalonia.Browser`.
3. Ensure browser can reach gateway URL and CORS is enabled.
