# Feature: Modelliert Systemwerte und Stationen zentral im Core

## Beschreibung

Dieses Feature beschreibt die zentralen Core-Modelle für Stationen und Systemwerte. Sie bilden die Grundlage für Konfiguration, Kommunikationszuordnung, Historien und datengetriebene Fachlogik.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CSystemValue.cs`
- `Core\Core\System\Values\CSystemValueExt.cs`
- `Core\Core\System\CStation.cs`
- `Core.Tests\Core\System\CStationTests.cs`

## Fachlicher Nutzen

- Stationen können global registriert und wiedergefunden werden
- Systemwerte tragen Kontext zu Station, Substation und Beschreibung
- Änderungs- und Refresh-Zeiten werden mitgeführt
- XML-Serialisierung unterstützt Persistenz und Austausch

## Beobachtete Abläufe

- `CStation` registriert neue Stationen in einer statischen Liste und löst Ereignisse aus.
- `CStationTests` prüfen Konstruktion, Benachrichtigung und XML-Ausgabe.
- `CSystemValue` speichert Wert, Beschreibung, Änderungszeit und Bit-/Typzugriffe.
- `CSystemValueExt` erweitert die Werte um Telegramm- und Datenbankbezug.

## Offene Fragen

- Ob die globale Stationsregistrierung langfristig durch eine Infrastruktur ersetzt werden soll
- Welche Teile der Systemwertlogik noch unvollständig implementiert sind
- Wie die XML- und Datenbankmodelle künftig vereinheitlicht werden sollten
