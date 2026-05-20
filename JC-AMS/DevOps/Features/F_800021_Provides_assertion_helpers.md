# Feature: Stellt Assertions-Hilfen bereit

## Beschreibung

Dieses Feature beschreibt die zentralen Assert-Hilfen für die Solution. Sie kapseln Prüfungen und verhalten sich je nach Zielplattform leicht unterschiedlich.

## Sichtbare technische Bausteine

- `Core\Core\SAssertions.cs`
- `Core.Tests\Core\SAssertionsTests.cs`

## Fachlicher Nutzen

- Gemeinsame Assertions können konsistent genutzt werden
- Debug-Verhalten lässt sich in Tests gezielt überprüfen
- Plattformabhängige Unterschiede bleiben gekapselt
- Fehler werden klar und reproduzierbar ausgelöst

## Beobachtete Abläufe

- `Assert(true)` erzeugt keine Ausgabe.
- `Assert(false)` löst je nach Plattform eine Exception oder Debug-Ausgabe aus.
- Zusätzliche Message- und Detailtexte werden unterstützt.
- Die Tests prüfen, dass keine unerwünschte Debug-Ausgabe entsteht.

## Offene Fragen

- Welche Assertion-Varianten im restlichen Code noch genutzt werden
- Ob zusätzliche Diagnoseinformationen standardisiert werden sollten
- Ob die Helpers in eine modernere Test-/Debug-Strategie überführt werden sollen
