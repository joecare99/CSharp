# Feature: Bietet eine Timeout-Hilfsklasse mit alter Timing-Semantik

## Beschreibung

Dieses Feature beschreibt `CTimeout` als veraltete Timeout-Hilfsklasse mit wählbarer Start- und Ablaufzeit.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CTimeout.cs`
- `System.DateTime`

## Fachlicher Nutzen

- Legacy-Code kann Zeitlimits ohne moderne Timer-API abbilden
- Timeout-Zustände sind lesbar und einfach zu prüfen
- Ein Delegate erlaubt Testbarkeit der aktuellen Zeit

## Beobachtete Abläufe

- `Seconds` und `ElapsedSeconds` setzen bzw. berechnen das Timeout über Millisekunden oder Sekunden.
- `IsTimeout` prüft, ob der Ablaufzeitpunkt erreicht ist.
- `Disable()` deaktiviert das Timeout vollständig.
- `Restart()` setzt den Ablaufpunkt auf Basis der gespeicherten Dauer zurück.

## Offene Fragen

- Ob diese Klasse nur noch aus Kompatibilitätsgründen dokumentiert werden muss
- Ob die inkonsistente Sekunden-/Millisekundenlogik noch gewartet werden soll
