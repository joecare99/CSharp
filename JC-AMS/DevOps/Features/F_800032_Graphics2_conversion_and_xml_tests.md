# Feature: Prüft Grafik-Konvertierungen und XML-Serialisierung

## Beschreibung

Dieses Feature beschreibt die Tests rund um `SGraphics2`. Sie validieren die Konvertierung von Farben, ColorCubes, Fonts, Pens und Punkten in Text- und XML-Form sowie die Rückkonvertierung aus Textwerten.

## Sichtbare technische Bausteine

- `Core.Tests\Core\SGraphics2Tests.cs`
- `Core\Core\SGraphics2.cs`
- `Core\Core\Components\Coloring\CColorCube`
- `System.Drawing`-Objekte wie `Color`, `Font`, `Pen` und `Point`

## Fachlicher Nutzen

- UI- und Konfigurationsobjekte lassen sich stabil in Text oder XML speichern
- Grafische Objektzustände können reproduzierbar verglichen werden
- Namens- und Farbzuordnung bleibt zwischen Laufzeit und Persistenz konsistent
- Diagnoseausgaben für Grafikelemente werden testbar

## Beobachtete Testinhalte

- Farben werden über bekannte Namen oder explizite ARGB-Werte geprüft.
- Fonts und Pens werden sowohl als Text als auch als XML getestet.
- Punkte und Rechtecke werden für Serialisierung und Deserialisierung abgesichert.
- Die Tests vergleichen auch bekannte Farben mit der Erkennung über `FindKnownColor()`.

## Offene Fragen

- Ob die XML-Hilfen noch um vollständige Deserialisierung ergänzt werden sollen
- Welche weiteren grafischen Typen im selben Stil getestet werden müssen
- Ob die Tests für plattformübergreifende Rendering-Unterschiede erweitert werden sollten
