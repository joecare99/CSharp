# Feature: Modelliert Substations und deren Lookup-Helfer

## Beschreibung

Dieses Feature beschreibt das Substation-Modell als Teil der Stationshierarchie sowie die dazugehörigen Lookup-Helfer.

## Sichtbare technische Bausteine

- `Core\Core\System\CSubStation.cs`
- `Core\Core\System\CStation.cs`
- `Core\Core\System\Values\CSystemValue.cs`
- `Core\Core\System\Values\CSystemValues.cs`

## Fachlicher Nutzen

- Stationsunterbereiche bleiben über Parent-Beziehungen verknüpft
- Substations tragen IDs und Beschreibungen für Systemwerte und Konfiguration
- Lookup-Methoden vereinheitlichen den Zugriff auf Substation-Objekte
- Die Struktur unterstützt die hierarchische Zuordnung von Systemwerten

## Beobachtete Abläufe

- `CSubStation` hält Substation-ID, Beschreibung und Parent-Station.
- `CSubStation.System` ist als bekannte System-Substation vorgesehen.
- Die Lookup-Methoden `GetSubstation(int)` und `GetSubstation(string)` sind als zentrale Einstiegspunkte vorgesehen.
- `CValue` und `CSystemValue` nutzen Substation-Bezüge zur Wertauflösung.

## Offene Fragen

- Wie die Lookup-Methoden intern die Registry auflösen sollen
- Ob die Substation-Registrierung und Initialisierung vollständig zentralisiert werden soll
