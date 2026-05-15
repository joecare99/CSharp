# Feature: Bietet Datumsformate und Kalenderwochenlogik

## Beschreibung

Dieses Feature beschreibt `SDateHelpers` als einfache Datums-Hilfsklasse für Anzeigeformate, Kalenderwochen und Parsing eines festen Datumsformats.

## Sichtbare technische Bausteine

- `Core\Core\SDateHelpers.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`
- `System.Globalization`

## Fachlicher Nutzen

- Datumswerte werden in anwendungsfreundliche Anzeigeformate gebracht
- Kalenderwochen können aus Datum und Kultur bestimmt werden
- Ein festes Zeitformat kann in `DateTime` geparst werden

## Beobachtete Abläufe

- `OrderFormat(...)` gibt nur Datum oder Datum+Zeit zurück.
- `DateOfFirstDayInWeek(...)` berechnet den Wochenanfang aus Jahr und KW.
- `DateTime_Sec(...)` liest Zeitstrings mit und ohne Millisekunden.
- `sDateTime_MSec(...)` und `sDateTime_Sec(...)` formatieren Datum und Uhrzeit.
- `KW(...)` verwendet die aktuelle Kultur zur Kalenderwochenbestimmung.

## Offene Fragen

- Ob die Wochenstartberechnung mit `DayOfWeek` überall fachlich korrekt ist
- Ob zusätzliche regionale Datumsformate dokumentiert werden sollten
