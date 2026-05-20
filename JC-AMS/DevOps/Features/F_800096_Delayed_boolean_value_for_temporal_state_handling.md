# Feature: Bietet einen verzögerten booleschen Wert für zeitabhängige Zustände

## Beschreibung

Dieses Feature beschreibt `CDelayedBool` als kleine Hilfsklasse für verzögert wirksam werdende boolesche Zustände.

## Sichtbare technische Bausteine

- `Core\Core\Components\CDelayedBool.cs`
- Zeitbasierte Logik über `DateTime`

## Fachlicher Nutzen

- Zustände können erst nach einer definierten Verzögerung als aktiv gelten
- Temporäre Wechsel werden verhindert oder geglättet
- Default- und Aktivzustände bleiben getrennt modellierbar

## Beobachtete Abläufe

- `Value` gibt abhängig von Zeit und Gültigkeit entweder den aktuellen oder den Defaultwert zurück.
- Beim Setzen wird die Gültigkeit entweder auf die Zukunft oder auf `DateTime.MaxValue` gesetzt.
- `Reset()` setzt Wert und Gültigkeit auf den Defaultzustand zurück.

## Offene Fragen

- Ob die Klasse für mehrere parallele Statusarten erweitert werden sollte
- Ob die Semantik von `ValueActive` weiter präzisiert werden muss
