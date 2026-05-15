# Feature: Steuert Logging-Infrastruktur und Ausgabeziele

## Beschreibung

Dieses Feature beschreibt `SLogging` als zentrale Logging-Infrastruktur des Core-Bereichs. Die Klasse routet Meldungen an Datei, Protocol oder EventLog und unterstützt Startup-Logging.

## Sichtbare technische Bausteine

- `Core\Core\Logging\SLogging.cs`
- `Core\Core\Logging\IProtocol.cs`
- `Core\Core\SFileHelpers.cs`
- `System.Diagnostics.EventLog`

## Fachlicher Nutzen

- Logausgaben werden zentral gesteuert
- Startup- und Laufzeitlog können getrennt behandelt werden
- Unterschiedliche Zielsysteme lassen sich einheitlich bedienen
- Fehler können optional an einen ErrorHandler weitergereicht werden

## Beobachtete Abläufe

- `DoWriteLog` kapselt Zielarten wie Datei, Stream, Protocol und EventLog.
- `Log(...)` berücksichtigt StartUp- und Filter-Topics.
- `DeleteStartupLog()` entfernt das Startup-Logfile.
- `DebugPrint(...)` schreibt zeitgestempelte Debug-Ausgaben.

## Offene Fragen

- Ob die Routing-Logik moderner abstrahiert werden sollte
- Ob die Log-Dateipfade künftig konfigurierbar statt fest verdrahtet sein sollen
