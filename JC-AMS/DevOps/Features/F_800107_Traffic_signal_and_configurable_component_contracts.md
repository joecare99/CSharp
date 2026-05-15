# Feature: Beschreibt kontraktbasierte Konfiguration und Verkehrslicht-Komponenten

## Beschreibung

Dieses Feature fasst die Core-Vertragsgrenzen für konfigurierbare Komponenten und Verkehrslicht-ähnliche UI-/Logikobjekte zusammen.

## Sichtbare technische Bausteine

- `Core\Core\Components\IConfigurable.cs`
- `Core\Core\Components\ITrafficSignal.cs`
- `Core\Core\Components\Coloring\CColorState.cs`
- `Core\Core\Components\CListOfObjects.cs`

## Fachlicher Nutzen

- Konfigurierbare Komponenten erhalten einen einheitlichen Lade-/Speichervertrag
- Verkehrslicht-Komponenten können Konfiguration, Rendering und Segmentbezug zusammenführen
- Die Schnittstellen dienen als Bauplan für UI- und Prozessobjekte

## Beobachtete Abläufe

- `IConfigurable` definiert `LoadConfiguration(int)` und `SaveConfiguration(int)`.
- `ITrafficSignal` erweitert den Vertrag um Rot/Gelb/Grün-Zustände, Segmentverwaltung und Zeichnen.
- `SetBySegmentList(...)`, `EnterSegment(...)`, `LeaveSegment(...)` und `JournalizeInOutState()` markieren die typische Signal-Logik.

## Offene Fragen

- Welche konkreten Implementierungen `ITrafficSignal` aktuell verwenden
- Ob die Konfigurationssignatur weiterhin nur über einen Index laufen sollte
