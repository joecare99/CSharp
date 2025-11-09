# ExtendedConsole

Supplemental abstractions and helpers built on top of `ConsoleLib` providing higher-level console services (`IExtendedConsole`), extended color / cursor utilities and convenience APIs required by richer console demos (e.g. PlaceAuthorityConsoleDemo).

## Purpose
Decouple application logic from the raw `System.Console` surface while still enabling richer features (window title, resizing reactions, buffered output) not covered by the core minimal `IConsole`.

## Key Features
- Extended output helpers (line writing, framed sections, status areas).
- Central place to evolve advanced console behaviors without bloating core `ConsoleLib`.
- Shared abstractions reused across multiple demos / tools.

## Targets
Multi-targeted: classic framework (net462+) and modern Windows targets (net6.0–net9.0).

## Usage
Usually consumed indirectly via DI:
```csharp
services.AddSingleton<IConsole, ConsoleProxy>();
services.AddSingleton<IExtendedConsole, ExtendedConsole>();
```
Then injected into an `Application` or view class.

## Design Notes
- Thin layer; keeps policy / formatting separate.
- Avoids large static helper classes – favors DI for testability.

## Contributing
Align with solution guidelines. Keep this assembly lean; major UI widgets belong in `ConsoleLib`.

## License
(Insert license here.)
