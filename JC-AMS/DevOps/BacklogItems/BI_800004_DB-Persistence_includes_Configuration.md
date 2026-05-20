# Backlog Item: DB-Persistence includes Configuration

## Ziel

Konfigurationsdaten sollen vollständig und nachvollziehbar persistent gemacht werden. Dazu zählen Verbindungsparameter, Instanzzuordnungen und Service-Konfigurationen.

## Inhalt

- Service- und Systemkonfiguration
- Verbindungsdaten
- Anlagen- und Produktionszellenzuordnung
- Start- und Laufzeitparameter

## Beobachtungen aus der Solution

- `Service\Service\CService.cs` speichert Hostdaten in die Konfiguration
- `MPS\MPS\CMPS.cs` liest Produktionszellen- und Instanzdaten beim Start
- `Core\Core\System` enthält mehrere Konfigurationsklassen für Teilsysteme

## Akzeptanzkriterien

- Konfiguration wird nach Neustart korrekt geladen
- Fehlende Werte werden protokolliert
- Konfigurationsdaten sind vom Betriebszustand getrennt
- Änderungen bleiben nachvollziehbar
