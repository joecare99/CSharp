# Feature: Bietet Datums-, Kalenderwochen- und Formatierhilfen

## Beschreibung

Dieses Feature beschreibt `SDateHelpers` als Sammlung einfacher Datumsfunktionen für Anzeige, Parsing und Kalenderwochenberechnung.

## Sichtbare technische Bausteine

- `Core\Core\SDateHelpers.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`
- `System.Globalization`

## Fachlicher Nutzen

- Datumswerte werden einheitlich für Anzeigen formatiert
- Kalenderwochen können kulturabhängig berechnet werden
- Zeichenketten im festen Datumsformat lassen sich in `DateTime` umwandeln
- Sekunden- und Millisekundenformate werden konsistent erzeugt

## Beobachtete Abläufe

- `OrderFormat(...)` gibt Datum oder Datum+Zeit zurück, abhängig vom Tagesanteil.
- `DateOfFirstDayInWeek(...)` berechnet den ersten Tag einer Kalenderwoche.
- `DateTime_Sec(...)` parst feste Datumsstrings mit oder ohne Millisekunden.
- `sDateTime_MSec(...)` und `sDateTime_Sec(...)` erzeugen formattierte Strings.
- `KW(...)` nutzt die aktuelle Kultur zur Kalenderwochenberechnung.

## Offene Fragen

- Ob die Parser für fehlerhafte Eingaben stärker abgesichert werden sollten
- Ob die Formatierungslogik für regionale Anforderungen erweitert werden muss
