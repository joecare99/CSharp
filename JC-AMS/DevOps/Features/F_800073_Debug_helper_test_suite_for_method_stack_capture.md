# Feature: Dokumentiert die Debug-Helper-Test-Suite für Methodenstacks

## Beschreibung

Dieses Feature beschreibt `SDebugHelpersTests` als sehr kleine Test-Suite für die Stack-Ausgabe der Debug-Hilfen.

## Sichtbare technische Bausteine

- `Core.Tests\Core\SDebugHelpersTests.cs`
- `Core\Core\SDebugHelpers.cs`
- `System.Diagnostics.StackTrace`

## Fachlicher Nutzen

- Der Methodenstack bleibt testbar und reproduzierbar
- Plattformabhängige Unterschiede zwischen .NET-Zielumgebungen werden im Test sichtbar

## Beobachtete Testinhalte

- Der Test vergleicht die erzeugte Stack-Ausgabe mit einer erwarteten, festen Zeichenkette.
- Es gibt unterschiedliche Erwartungen für .NET 6 und ältere Zielumgebungen.
- Die Testklasse ist bewusst klein und fokussiert.

## Offene Fragen

- Ob zusätzliche Debug-Helfer in denselben Testbereich aufgenommen werden sollen
- Ob die erwarteten Stackketten für weitere Plattformen ergänzt werden müssen
