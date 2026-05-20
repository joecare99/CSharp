# Feature: Verwalten von UI-Farbpaletten mit ColorList und ColorCube

## Beschreibung

Dieses Feature beschreibt `CColorList` und `CColorCube` als Grundlage für UI-Farbpaletten, Brush-/Pen-Erzeugung und Konfigurationspersistenz.

## Sichtbare technische Bausteine

- `Core\Core\Components\Coloring\CColorList.cs`
- `Core\Core\Components\Coloring\CColorCube.cs`
- `Core.Tests\Core\Components\Coloring\CColorListTests.cs`
- `Core.Tests\Core\Components\Coloring\CColorCubeTests.cs`

## Fachlicher Nutzen

- Farbpaletten können zentral erstellt, verglichen und gespeichert werden
- Brushes und Pens stehen direkt für UI-Rendering zur Verfügung
- Layout- und Zustandsfarben bleiben konfigurierbar

## Beobachtete Abläufe

- `CColorList` hält Farben, Brushes und Pens parallel in Arrays.
- `SaveConfiguration(...)` und `LoadConfiguration(...)` binden Farben an Konfigurationswerte.
- `Equals(...)` vergleicht Listenlänge, Farben und Label.
- `CColorCube`-Tests zeigen Factory- und Konstruktorverhalten, während viele weitere Teststellen noch offen sind.

## Offene Fragen

- Ob die Dispose-/Equals-/XML-Funktionen der Tests noch produktive Lücken abdecken
- Ob die Palette-Objekte künftig stärker an moderne Ressourcenverwaltung angepasst werden sollten
