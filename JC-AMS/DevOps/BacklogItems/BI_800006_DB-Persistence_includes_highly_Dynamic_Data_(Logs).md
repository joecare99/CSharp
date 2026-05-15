# Backlog Item: DB-Persistence includes highly Dynamic Data (Logs)

## Ziel

Hochdynamische Daten wie Protokolle, Fehler und Diagnoseinformationen sollen dauerhaft gespeichert werden, ohne den Betrieb zu stören.

## Inhalt

- Protokolle und Fehlerhistorie
- Diagnose- und Monitoringdaten
- Service- und Kommunikationsmeldungen
- Eventlog- und Dateilog-Ausgaben

## Beobachtungen aus der Solution

- `Core\Core\Logging` enthält Protokoll- und Fehlerklassen
- `Service\Service\CService.cs` schreibt Fehler zusätzlich in Logdateien
- `MPS\MPS\CMPS.cs` loggt Import- und Verarbeitungszustände
- `WinGUI\WinGUI` enthält Diagnose- und Analyseformulare

## Akzeptanzkriterien

- Logdaten gehen bei hoher Last nicht verloren
- Fehler sind zeitlich eindeutig zuordenbar
- Logs können getrennt von fachlichen Stammdaten ausgewertet werden
- Speicherorte und Formate sind dokumentiert
