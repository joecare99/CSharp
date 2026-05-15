# Feature: Modelliert Kommunikationsstationen und Substations aus der Datenbank

## Beschreibung

Dieses Feature beschreibt die Datenbankmodelle für Kommunikationsstationen und Kommunikations-Substations im CommSystem-Bereich.

## Sichtbare technische Bausteine

- `Core\Core\CommSystem\Communicator\CCommunicationStation.cs`
- `Core\Core\CommSystem\Communicator\CCommunicationStations.cs`
- `Core\Core\CommSystem\Communicator\CCommunicationSubStation.cs`
- `Core\Core\CommSystem\Communicator\CCommunicationSubStations.cs`
- `Core\Core\SQL\CSQLQuery.cs`

## Fachlicher Nutzen

- Kommunikationsstationen werden aus der Datenbank geladen
- Substations bleiben mit Stationen verknüpft
- Die Modelle dienen als Grundlage für kommunikative Zuordnung und Routing

## Beobachtete Abläufe

- `CCommunicationStation` liest Station, Beschreibung, Stationstyp und SPS-Referenz aus einem Query-Objekt.
- `CCommunicationStations` lädt alle Stationen statisch aus `System_Station`.
- `CCommunicationSubStation` liest Substation-IDs, Beschreibung und Parent-Station aus einem Query-Objekt.
- `CCommunicationSubStations` lädt alle Substations statisch aus `System_SubStation`.

## Offene Fragen

- Ob diese Modellklassen künftig durch gemeinsame Registry- oder Repository-Schichten ersetzt werden sollen
- Ob die statische Initialisierung bei Datenbankfehlern besser abgesichert werden muss
