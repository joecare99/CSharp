# Feature: Verarbeitet DNC-Programme und Werkzeugdaten

## Beschreibung

Dieses Feature bündelt die DNC-bezogenen Funktionen der Solution. Es umfasst Maschinen, NC-Programme, Werkzeuglisten, Exportfunktionen und die Anbindung externer oder interner DNC-Provider.

## Sichtbare technische Bausteine

- `DNC\DNC\CDNC.cs`: Initialisierung, Maschinen- und Gruppenverwaltung, Exportfunktionen
- `DNC\DNC\CNCProgramPool.cs`, `CNCProgram.cs`, `CNCMachine.cs`: fachliche DNC-Objekte
- `DNC\DNC\Manager`: DNC-Manager und Konfiguration
- `Core\Core\DNC`: gemeinsame DNC-Typen und Providerlogik

## Fachlicher Nutzen

- NC-Programme können verwaltet und exportiert werden
- Werkzeugdaten können für Maschinen und Listen aufbereitet werden
- DNC-Provider lassen sich umschalten oder extern anbinden
- Maschinenkonfiguration und Programmbestand bleiben nachvollziehbar

## Offene Fragen

- Welche DNC-Provider produktiv eingesetzt werden
- Wie Programme versioniert und exportiert werden
- Welche Teile der Werkzeugverwaltung extern oder intern gepflegt werden
