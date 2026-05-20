# Feature: Bietet Timing-Hilfen für Blinken, Dreiecksgenerator und aktuelle Zeitdarstellung

## Beschreibung

Dieses Feature beschreibt `CTiming` als statische Timing-Hilfsklasse für Blinkzustände, Alphageneratoren und formatierte aktuelle Zeitstrings.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CTiming.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Blinkzustände können zeitabhängig berechnet werden
- Dreiecksgeneratoren liefern eine einfache zeitproportionale Alpha-Logik
- Aktuelle Zeit kann in Sekundengenauigkeit oder mit Millisekunden formatiert werden

## Beobachtete Abläufe

- `BlinkState` nutzt einen 1000-ms-Takt.
- `ErrorAlpha` erzeugt einen Dreieckswert auf Basis eines Intervalls.
- `sDateTimeNow_MSec` und `sDateTimeNow_Sec` liefern formatierte Zeitstrings.
- Ein Delegate erlaubt das Austauschen der Tageszeit für Tests.

## Offene Fragen

- Ob die Hilfsklasse in klarere Teilbereiche aufgeteilt werden sollte
- Ob die statische Zustandsabhängigkeit für Tests und Parallelität ausreichend ist
