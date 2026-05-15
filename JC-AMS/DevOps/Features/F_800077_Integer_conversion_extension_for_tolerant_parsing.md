# Feature: Bietet eine integerbasierte Konvertierungs-Extension mit toleranter Parsing-Logik

## Beschreibung

Dieses Feature beschreibt `SAsIntXtntn` als Konvertierungsschicht für `int`, `long` und die Erkennung integerähnlicher Strings.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsIntXtntn.cs`
- `Core.Tests\Core\Extensions\SAsIntXtntnTests.cs`
- `System.Text.RegularExpressions`

## Fachlicher Nutzen

- Heterogene Werte können robust in Ganzzahlen umgewandelt werden
- String-Eingaben werden tolerant, aber kontrolliert behandelt
- Die Logik unterstützt auch robuste Defaultwerte bei ungültigen Eingaben

## Beobachtete Abläufe

- `AsInt32(string)` und `AsInt64(string)` parsen Strings direkt und liefern sonst 0.
- `AsInt32(object)` und `AsInt64(object)` arbeiten über `Convert` und Fallbacks.
- `IsInt(...)` prüft String-Eingaben mit Regex auf Ganzzahllogik.
- Die Tests decken `null`, `DBNull`, Grenzen, Fließkommawerte und Sonderzeichen ab.

## Offene Fragen

- Ob die Stillschweigen-auf-0-Strategie für Fehlerfälle ausreichend ist
- Ob Hex- oder kulturabhängige Integerformate noch unterstützt werden sollen
