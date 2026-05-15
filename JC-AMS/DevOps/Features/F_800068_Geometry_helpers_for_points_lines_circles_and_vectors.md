# Feature: Bietet Geometrie-Hilfen für Punkte, Linien, Kreise und Vektoren

## Beschreibung

Dieses Feature beschreibt `SMath2` als Geometrie-Hilfsklasse für Punkt-/Vektor-Konvertierungen, Kreisberechnungen, Distanzen und Schnittpunkte.

## Sichtbare technische Bausteine

- `Core\Core\Math2\SMath2.cs`
- `Core.Tests\Core\Math2\SMath2Tests.cs`
- `System.Drawing`
- `System.Numerics.Vector2`

## Fachlicher Nutzen

- Geometrische Hilfsfunktionen werden zentral und wiederverwendbar bereitgestellt
- Punkte und Vektoren können zwischen Framework-Typen konvertiert werden
- Linien-, Kreis- und Distanzberechnungen lassen sich im UI- und Maschinenkontext nutzen
- Undefined-Sentinels unterstützen robuste Fehler- und Leerwertbehandlung

## Beobachtete Abläufe

- `IsUndefined(...)` und die Sentinel-Properties markieren ungültige Geometrieobjekte.
- `AbsArcLength(...)`, `ArcLength(...)` und `Angle(...)` bedienen Bogen- und Winkelberechnungen.
- `BaseOfPointOrthographicToStraight(...)`, `DistancePointSegment(...)` und `DistancePointStraight(...)` berechnen orthogonale Bezüge.
- `CircleCenter(...)`, `CircleIntersectionPoint(...)` und `CircleIntersectionStraight(...)` berechnen Kreise und Schnittpunkte.
- `ToPointF(...)`, `ToPoint(...)` und `Vector2al(...)` konvertieren zwischen Geometrie-Typen.

## Offene Fragen

- Ob die große Geometrieklasse in kleinere Domänenbausteine aufgeteilt werden sollte
- Ob die unvollständigen Testfälle künftig ergänzt oder als technische Schulden dokumentiert werden müssen
