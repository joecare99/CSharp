# Feature: Stellt Debug- und Stacktrace-Hilfen bereit

## Beschreibung

Dieses Feature beschreibt die Hilfen zum Ermitteln von Method-Stacks und zur Unterstützung von Debug-Ausgaben. Sie werden vor allem für Diagnose und Testverhalten genutzt.

## Sichtbare technische Bausteine

- `Core\Core\SDebugHelpers.cs`
- `Core.Tests\Core\SDebugHelpersTests.cs`

## Fachlicher Nutzen

- Diagnoseinformationen können gezielt extrahiert werden
- Plattformabhängige Stacktraces lassen sich vergleichen
- Fehlersuche und Testvalidierung werden erleichtert

## Beobachtete Abläufe

- `GetMethodStack()` liefert einen kompakten Method-Stack-Ausschnitt.
- Der erwartete Stack unterscheidet sich je nach Zielplattform.
- Die Tests prüfen bewusst die unterschiedliche Runtime-Form.

## Offene Fragen

- Ob die Stacktrace-Ausgabe weiter vereinheitlicht werden soll
- Welche Debug-Helfer im restlichen Code noch fehlen
- Ob die Diagnoseausgabe modernisiert oder gekapselt werden sollte
