# Feature: Erkennt .NET-Runtime- und Framework-Versionen über die Registry

## Beschreibung

Dieses Feature beschreibt `SDotNetUtil` als Hilfsklasse zur Abfrage installierter .NET-Framework-Versionen aus der Windows-Registry.

## Sichtbare technische Bausteine

- `Core\Core\SDotNetUtil.cs`
- `Microsoft.Win32.Registry`
- `Core.Tests\Core\SDotNetUtilTests.cs`

## Fachlicher Nutzen

- Installierte Framework-Versionen können zur Laufzeit geprüft werden
- Kompatibilitätsprüfungen lassen sich zentral vornehmen
- Die Funktion unterstützt Diagnose und Setup-Validierung

## Beobachtete Abläufe

- `CheckDotNETVersion(...)` vergleicht Framework- und Service-Pack-Werte.
- `GetDotNETVersion(...)` liefert die höchste gefundene Version zurück.
- `GetDotNETVersions(...)` liest den Registry-Zweig `SOFTWARE\Microsoft\NET Framework Setup\NDP`.
- Die Tests prüfen erwartete Mindest- und Vergleichswerte.

## Offene Fragen

- Ob die Registry-Abfrage für neuere .NET-Laufzeiten ebenfalls erweitert werden sollte
- Ob Fehlerfälle bei fehlenden Registry-Schlüsseln weiter differenziert behandelt werden sollen
