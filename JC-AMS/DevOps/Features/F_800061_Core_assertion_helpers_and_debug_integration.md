# Feature: Bietet Core-Assertions und deren Debug-Integration

## Beschreibung

Dieses Feature beschreibt `SAssertions` als sehr kleine Assertion-Hilfsklasse auf Basis von `System.Diagnostics.Debug.Assert`.

## Sichtbare technische Bausteine

- `Core\Core\SAssertions.cs`
- `Core.Tests\Core\SAssertionsTests.cs`
- `System.Diagnostics.Debug`

## Fachlicher Nutzen

- Einfache Debug-Assertions bleiben zentral verfügbar
- Debug- und Testverhalten sind über Tests nachvollziehbar
- Die Hilfen sind bewusst minimal und direkt

## Beobachtete Abläufe

- `Assert(bool)` ruft `Debug.Assert(Condition)` auf.
- Die überladenen Varianten unterstützen Message- und DetailMessage-Texte.
- Die Tests prüfen, dass die Aufrufe ohne unerwünschte Nebenwirkungen durchlaufen.

## Offene Fragen

- Ob die Assertion-Hilfen noch weiter ausgebaut werden sollen
- Ob das Testverhalten für neuere Zielplattformen anders dokumentiert werden muss
