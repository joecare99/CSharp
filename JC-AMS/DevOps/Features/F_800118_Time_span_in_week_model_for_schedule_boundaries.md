# Feature: Modelliert Zeitspannen innerhalb einer Woche

## Beschreibung

Dieses Feature beschreibt `CTimeSpanInWeek` als Datenmodell für Zeitfenster innerhalb einer Woche mit Text- und Datumssichten.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CTimeSpanInWeek.cs`
- `System.TimeSpan`

## Fachlicher Nutzen

- Wochenbezogene Zeitfenster lassen sich kompakt speichern
- Start- und Endzeiten sind sowohl als Text als auch als Datum sichtbar
- Die Klasse dient als Baustein für Produktivitäts- und Zeitplanmodelle

## Beobachtete Abläufe

- Der Konstruktor parst Begin- und Endwerte aus Strings.
- `sBegin` und `sEnd` formatieren die Zeitspannen als lesbaren Text.
- `dtBegin` und `dtEnd` projizieren die Zeitspanne auf den letzten Sonntag-Mitternachtspunkt.

## Offene Fragen

- Ob die Parserlogik für ungültige Eingaben ausreichend ist
- Ob eine stärkere Validierung für Begin/End-Reihenfolgen nötig ist
