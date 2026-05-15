# Backlog Item: DB-Persistence includes Dynamic Data

## Ziel

Dynamische Betriebsdaten sollen konsistent gespeichert werden, damit ein reproduzierbarer Anlagenzustand möglich ist.

## Inhalt

- Aufträge und Produktionsfortschritt
- Paletten, Werkstücke und Werkzeuge
- Plätze, Magazine und Produktionszellen
- NC-Programme und Fertigungszustände

## Beobachtungen aus der Solution

- `MPS\MPS\CProductionCellPool`, `CPalletPool`, `CFixturePool` und `CNCProgramPool` werden initialisiert
- `MPS\MPS\CMPS.cs` verarbeitet Importdaten und aktualisiert Produktionslogik
- `Core\Core\MPS` enthält zahlreiche Domänenobjekte für Workflows und Zustände

## Akzeptanzkriterien

- Dynamische Daten werden konsistent gelesen und geschrieben
- Zuordnungen zwischen Aufträgen, Paletten und Werkstücken bleiben erhalten
- Änderungen werden bei Refresh- und Importvorgängen korrekt übernommen
