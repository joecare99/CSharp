# Feature: Konfiguriert Farbzustände und Paletten für UI-States

## Beschreibung

Dieses Feature beschreibt `CColorState` und `CColorDef` als Farb- und Palette-Bausteine für UI-Zustände und Darstellungskonventionen.

## Sichtbare technische Bausteine

- `Core\Core\Components\Coloring\CColorState.cs`
- `Core\Core\Components\Coloring\CColorDef.cs`
- `Core\Core\Components\Coloring\CColorList.cs`
- `Core\Core\Components\Coloring\CColorCube.cs`

## Fachlicher Nutzen

- UI-Zustände können über benannte Farbsets dargestellt werden
- Brushes und Pens werden zentral aus Farbdefinitionen abgeleitet
- Unterschiedliche visuelle Zustände bleiben konsistent konfigurierbar

## Beobachtete Abläufe

- `CColorState` bietet Eigenschaften für Normal-, Highlight-, Selected- und Inactive-Zustände.
- Die Klasse leitet Brushes und Pens in mehreren Stärken aus den Listen ab.
- `LoadConfiguration(...)` und `SaveConfiguration(...)` binden die Farbzustände an Konfigurationswerte.
- `CColorDef` stellt eine große Sammlung statischer Farb- und Brush-Konstanten bereit.

## Offene Fragen

- Ob die vielen statischen Farbkonstanten künftig zentralisiert werden sollten
- Ob die Palette- und Brush-Lebenszyklen in der heutigen Architektur noch passend sind
