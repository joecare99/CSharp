# Feature: Stellt Logging- und Startup-Protokollierung bereit

## Beschreibung

Dieses Feature beschreibt die zentrale Protokollierungs- und Startup-Logik der Solution. Sie verarbeitet Debug-Ausgaben, Startup-Logs, EventLog-Ausgaben und Datei- oder Stream-basierte Logs.

## Sichtbare technische Bausteine

- `Core\Core\Logging\SLogging.cs`: zentrale Log-Fassade mit Datei-, Stream- und EventLog-Ausgabe
- `Core\Core\SFileHelpers.cs`: Datei- und Verzeichnisoperationen für Logging und Infrastruktur
- `Core.Tests\Core\Logging\SLoggingTests.cs`: vorhandene Testbasis für Logging-Verhalten

## Fachlicher Nutzen

- Startup- und Laufzeitfehler werden nachvollziehbar protokolliert
- Logging-Ziele können je nach Umgebung unterschiedlich angesprochen werden
- Datei- und EventLog-Ausgaben bleiben über eine zentrale Fassade gekapselt
- Fehlerzustände können gezielt an Zusatzmechanismen weitergeleitet werden

## Beobachtete Abläufe

- Während des Starts werden Einträge in eine Startup-Logdatei geschrieben.
- Bei gesetztem EventLog wird zusätzlich an das Windows-EventLog geschrieben.
- Debug-Ausgaben bleiben getrennt von fachlicher Protokollierung.
- Dateipfade und Verzeichnisse werden vor dem Schreiben geprüft oder angelegt.

## Offene Fragen

- Welche Logging-Profile produktiv verwendet werden
- Welche Logziele in welchen Betriebsarten aktiv sind
- Wie stark Startup- und Laufzeitlogs voneinander getrennt bleiben sollen
