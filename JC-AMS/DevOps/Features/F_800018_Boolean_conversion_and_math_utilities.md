# Feature: Bietet Boolean- und mathematische Hilfsfunktionen

## Beschreibung

Dieses Feature beschreibt zwei kleine, aber breit nutzbare Utility-Bereiche: die robuste Konvertierung beliebiger Werte in Boolean-Werte sowie mathematische Standardfunktionen mit zugehörigen Tests.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsBoolXtntn.cs`
- `Core.Tests\Core\Extensions\SAsBoolXtntnTests.cs`
- `Core\Core\Math2\SMath.cs`
- `Core.Tests\Core\Math2\SMathTests.cs`

## Fachlicher Nutzen

- Heterogene Eingabewerte können robust auf Wahrheitswerte abgebildet werden
- Zahlen und numerische Reihen können für Statistik- und Qualitätsfunktionen verwendet werden
- Standardformeln wie Mittelwert oder Cp werden zentral verfügbar gemacht
- Tests sichern die erwarteten Randfälle und typischen Eingaben ab

## Beobachtete Abläufe

- `AsBool()` akzeptiert `null`, `bool`, Textwerte und numerische Repräsentationen.
- `SMathTests` dokumentieren die erwarteten Ergebnisse für Mittelwert- und Cp-Berechnungen.
- Mehrere Datenreihenfälle werden mit parametrisierten Tests abgedeckt.
- Die Tests zeigen, dass die Utilities auf Grenzwerte und Sonderfälle ausgelegt sind.

## Offene Fragen

- Ob die Bool-Konvertierung sprachspezifische Werte wie JA/NEIN künftig explizit unterstützen soll
- Welche weiteren Statistikfunktionen neben Mittelwert und Cp fachlich relevant sind
- Ob die mathematischen Hilfen in eine konsistentere Utility-Struktur überführt werden sollen
