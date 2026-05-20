# Feature: Bietet Core-Assertions für Debug-Prüfungen

## Beschreibung

Dieses Feature beschreibt `SAssertions` als minimale Wrapper-Klasse um `Debug.Assert`.

## Sichtbare technische Bausteine

- `Core\Core\SAssertions.cs`
- `System.Diagnostics.Debug`

## Fachlicher Nutzen

- Debug-Assertions bleiben an einer zentralen Stelle verfügbar
- Die API ist bewusst klein und leicht in Legacy-Code einsetzbar

## Beobachtete Abläufe

- `Assert(bool)` ruft direkt `Debug.Assert(Condition)` auf.
- Die überladenen Varianten unterstützen Message und DetailMessage.

## Offene Fragen

- Ob die Assertions-Hilfen in Zukunft um Logging oder Exceptions ergänzt werden sollen
- Ob die Klasse für moderne Test- und Diagnosepfade erweitert werden muss
