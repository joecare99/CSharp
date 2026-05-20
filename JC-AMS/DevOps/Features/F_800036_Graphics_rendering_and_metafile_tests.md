# Feature: Prüft Grafik-Rendering und Metafile-Ausgabe

## Beschreibung

Dieses Feature beschreibt die umfangreichen Rendering-Tests für `SGraphics`. Sie validieren die grafische Ausgabe von Traffic Lights, Lade-/Entladeanzeigen, Sicherheits- und Signalzuständen sowie Text- und Zeichenoperationen im Metafile-Kontext.

## Sichtbare technische Bausteine

- `Core.Tests\Core\SGraphicsTests.cs`
- `Core\Core\SGraphics.cs`
- `System.Drawing`-basierte Renderingobjekte
- Metafile- und XML-/Binärprotokollierung im Test

## Fachlicher Nutzen

- Grafische Zustände werden reproduzierbar geprüft
- UI-Symbole für Anlage, Signale und Aktionen bleiben konsistent
- Rendering-Unterschiede zwischen Zuständen sind testbar
- Die Metafile-Ausgabe kann als stabiler Diagnosekanal dienen

## Beobachtete Testinhalte

- Traffic-Light- und Traffic-Signal-Darstellungen werden in mehreren Zustandskombinationen geprüft.
- Lade-/Entladeindikatoren werden mit Blink- und Richtungslogik getestet.
- Zusätzliche Zeichentests prüfen weitere Renderhilfen und deren Metafile-Struktur.
- Die Tests nutzen sehr große, protokollierte Referenzstrings für Bildvergleiche.

## Offene Fragen

- Welche Renderhilfen noch in der Produktion eingesetzt werden
- Ob die sehr großen Referenzdaten weiter gepflegt oder reduziert werden sollen
- Welche zusätzlichen Zustände und Symbole noch fehlen
