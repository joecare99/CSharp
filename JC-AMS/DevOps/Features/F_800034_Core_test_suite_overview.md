# Feature: Gibt eine Übersicht über die Core-Test-Suite

## Beschreibung

Dieses Feature beschreibt die allgemeine Testlandschaft des Core-Projekts. Die Testprojekte sichern Erweiterungen, Helper, Logging, Farben, Math, Assertions und Systemfunktionen ab.

## Sichtbare technische Bausteine

- `Core.Tests\Core\*`
- Kernklassen aus `Core\Core\*`

## Fachlicher Nutzen

- Wichtige Kernhilfen werden gegen Regressionen abgesichert
- Zielplattformunterschiede bleiben in den Tests sichtbar
- Einzelne Utility-Bereiche lassen sich getrennt erweitern
- Die Testbasis zeigt, welche Bereiche bereits stabilisiert sind und welche noch Platzhalter enthalten

## Beobachtete Testschwerpunkte

- Logging und Startup-Verhalten
- String-, Integer-, Bool- und numerische Erweiterungen
- Grafik- und XML-Konvertierung
- Datums-, Netzwerk- und Debug-Hilfen
- Assertions, Cipher und Systemwerte

## Offene Fragen

- Welche Testklassen noch Platzhalter enthalten und priorisiert werden sollten
- Ob eine Aufteilung in fachliche Testcluster sinnvoll wäre
- Welche Tests für .NET Framework 4.8 und .NET 6 separat behandelt werden müssen
