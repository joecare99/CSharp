# WinUI .NET 10 Migration

## Backlog Item
Migration der Projekte `UWP_00_Test` und `App2` auf `net10.0-windows10.0.19041.0`.

## Aufgaben
- [x] Ziel-Framework von `UWP_00_Test` auf .NET 10 anheben
- [x] Ziel-Framework von `App2` auf .NET 10 anheben
- [x] Default-Platform in `App2` setzen, damit Builds nicht auf `AnyCPU` fallen
- [x] Beide Projekte per Build validieren

## Ergebnis
- `UWP_00_Test` baut erfolgreich unter `net10.0-windows10.0.19041.0`.
- `App2` baut erfolgreich unter `net10.0-windows10.0.19041.0` mit `win-x86` als abgeleitetem Standard-RuntimeIdentifier.
- Die bestehenden Codeanalysewarnungen in `UWP_00_Test` wurden nicht verändert, da sie nicht migrationsblockierend waren.

## Hinweise
- `Microsoft.WindowsAppSDK` 2.2.0 ist bereits aktuell.
- `Microsoft.Windows.SDK.BuildTools` 10.0.28000.1839 ist bereits aktuell.
