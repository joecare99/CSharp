# Feature: Verwaltet Farb-Listen mit Brush-, Pen- und Konfigurations-Lifecycle

## Beschreibung

Dieses Feature beschreibt `CColorList` als Basis für farbige UI-Elemente mit paralleler Verwaltung von Color-, Brush- und Pen-Objekten sowie Konfigurationspersistenz.

## Sichtbare technische Bausteine

- `Core\Core\Components\Coloring\CColorList.cs`
- `Core\Core\SVariableHandling.cs`
- `System.Drawing`

## Fachlicher Nutzen

- Farbwerte können zentral gespeichert und geladen werden
- Brushes und Pens stehen passend zur Farbe sofort bereit
- Die Klasse eignet sich als Basismodell für UI-Farbpaletten

## Beobachtete Abläufe

- Der Konstruktor legt Color-, Brush- und Pen-Arrays parallel an.
- `Dispose()` gibt die generierten Brush- und Pen-Ressourcen wieder frei.
- `SaveConfiguration(...)` und `LoadConfiguration(...)` speichern bzw. lesen Farben per Konfigurationsschlüssel.
- `Equals(...)` vergleicht Label und Farbwerte.

## Offene Fragen

- Ob die manuelle Ressourcenverwaltung mit `Dispose()` für moderne UI-Patterns noch zeitgemäß ist
- Ob die Konfigurationsschlüssel weiter standardisiert werden sollten
