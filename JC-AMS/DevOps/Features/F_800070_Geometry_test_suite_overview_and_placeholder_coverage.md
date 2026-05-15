# Feature: Beschreibt die Geometrie-Test-Suite mit gemischter Abdeckung

## Beschreibung

Dieses Feature dokumentiert `SMath2Tests` als Test-Suite für die Geometrie-Hilfen. Die Suite enthält sowohl echte Prüfungen als auch viele Platzhalter, die den zukünftigen Ausbaubedarf sichtbar machen.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Math2\SMath2Tests.cs`
- `Core\Core\Math2\SMath2.cs`
- `System.Drawing`
- `System.Numerics`

## Fachlicher Nutzen

- Geometrische Konvertierungen werden bereits abgesichert
- Umfang und Lücken der Testabdeckung sind klar erkennbar
- Platzhalter markieren offene Aufgaben für geometrische Spezialfälle

## Beobachtete Testinhalte

- `ToPointF(...)`, `ToPoint(...)` und `Vector2al(...)` sind mit Datenfällen belegt.
- Viele Geometrie-Methoden wie Schnittpunkte, Kreismittelpunkte und Rotationen sind noch als `Assert.Fail()` markiert.
- Die Suite zeigt damit die beabsichtigte fachliche Bandbreite, nicht nur den aktuellen Implementierungsstand.

## Offene Fragen

- Welche geometrischen Spezialfälle als nächstes mit realen Tests gefüllt werden sollen
- Ob die Testdatei in mehrere thematische Klassen zerlegt werden sollte
