# Feature: Prüft Stationsserialisierung und Stationsbenachrichtigungen

## Beschreibung

Dieses Feature beschreibt die Tests rund um `CStation`. Sie validieren die Erzeugung von Stationen, die Benachrichtigung bei neuen Stationen und die XML-Serialisierung von Stationen mit ValueDefs und SubStations.

## Sichtbare technische Bausteine

- `Core.Tests\Core\System\CStationTests.cs`
- `Core\Core\System\CStation.cs`
- `Core\Core\System\Values\CSystemValueDef`
- `Core\Core\System\CSubStation`

## Fachlicher Nutzen

- Stationen werden mit konsistenten IDs und Beschreibungen angelegt
- Neue Stationen lösen erwartete Benachrichtigungen aus
- XML-Export bleibt mit Value- und SubStation-Struktur kompatibel
- Der Objektgraph der Station wird reproduzierbar serialisiert

## Beobachtete Testinhalte

- Der Setup-Test prüft die Basisinitialisierung und die `OnNewStation`-Benachrichtigung.
- XML-Tests vergleichen die komplette Serialisierung von Stationen mit unterschiedlichen ValueDefs und SubStations.
- Die Testdaten dokumentieren mehrere Stationsvarianten wie AGV und Loading.

## Offene Fragen

- Ob weitere Stationstypen und Sonderfälle zusätzlich abgesichert werden sollen
- Welche Teile der Stationslogik außerhalb der Serialisierung noch Testabdeckung benötigen
- Ob die Benachrichtigungssignatur langfristig vereinheitlicht werden sollte
