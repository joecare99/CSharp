# Feature: Stellt Grafik- und XML-Konvertierungen für UI-Assets bereit

## Beschreibung

Dieses Feature beschreibt die Grafik-Hilfen für Darstellung, Farbkonvertierung und XML-Serialisierung von UI-Objekten wie Farben, Stiften, Schriftarten, Punkten und Rechtecken.

## Sichtbare technische Bausteine

- `Core\Core\SGraphics.cs`
- `Core\Core\SGraphics2.cs`
- `Core.Tests\Core\SGraphicsTests.cs`

## Fachlicher Nutzen

- UI-Elemente können grafisch einheitlich dargestellt werden
- Farben, Fonts und Formen lassen sich serialisieren und wiederherstellen
- Metafile- und Protokollausgaben unterstützen Diagnose und Tests
- Grafische Hilfen stehen sowohl für Laufzeit- als auch Testkontext bereit

## Beobachtete Abläufe

- `SGraphics` zeichnet Zustände wie Ampeln, Lade-/Entladehinweise und AGV-Pfeile.
- `SGraphics2` konvertiert Farben, Fonts, Pens und Punkte in Text oder XML.
- Testcode analysiert Metafile-Ausgaben und rendert grafische Zustände in reproduzierbarer Form.
- Der Grafikbereich ist stark auf WinForms-/GDI+-Darstellung ausgerichtet.

## Offene Fragen

- Welche grafischen Helfer im UI tatsächlich produktiv verwendet werden
- Ob die XML-Konvertierung noch in allen Fällen kompatibel genug ist
- Welche Teile der Grafikausgabe noch Testabdeckung benötigen
