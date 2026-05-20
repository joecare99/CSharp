# Feature: Nutzt Komponenten, Dateninterfaces und Handler-Pools

## Beschreibung

Dieses Feature beschreibt die gemeinsame Komponentenebene der Solution. Dort werden Dateninterfaces, Handler-Pools, Transport- und Förderlogik sowie AGV-bezogene Fachobjekte zusammengeführt.

## Sichtbare technische Bausteine

- `Components\Components\CDataInterface.cs`: Aufbau, Start und Zuordnung von Import- und Exportschnittstellen
- `Components\Components\CHandlerPool.cs`: Verwaltung von Handlern, Fahrzeugen und Zuständen
- `Components\Components\CComponents.cs`: gemeinsame Komponenten-Konfigurationskennung
- zahlreiche `CHandler*`, `CTrack*`, `CStorage*`, `CFeeder*`-Klassen für Spezialfälle

## Fachlicher Nutzen

- Schnittstellen werden zentral und wiederverwendbar verwaltet
- Fahrzeug- und Anlagenzustände lassen sich in Pools organisieren
- Import-/Exportwege können über Dateninterface-Komponenten getrennt behandelt werden
- Transport- und Materialflusslogik bleibt strukturiert

## Offene Fragen

- Welche Komponenten produktiv im Einsatz sind
- Welche Dateninterface-Purposes primär genutzt werden
- Wie die Handler-Pools mit AGV-, MPS- und PLC-Logik zusammenspielen
