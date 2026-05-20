# Feature: Stellt Datums-Hilfen für Formatierung und Kalenderwochen bereit

## Beschreibung

Dieses Feature beschreibt die Datums-Hilfsfunktionen für Formatierung, Parsing, Kalenderwochen und Wochenanfänge.

## Sichtbare technische Bausteine

- `Core\Core\SDateHelpers.cs`
- mögliche zugehörige Testabdeckung im Core-Testbereich

## Fachlicher Nutzen

- Datumswerte können konsistent formatiert werden
- Kalenderspezifische Berechnungen wie KW und Wochenanfang werden zentral bereitgestellt
- Datumsstrings mit und ohne Millisekunden lassen sich in `DateTime` überführen
- UI- und Protokollschichten erhalten einheitliche Datumsausgaben

## Beobachtete Abläufe

- `OrderFormat()` gibt leere Werte für `DateTime.MinValue` zurück und kürzt reine Datumswerte ohne Uhrzeit.
- `DateTime_Sec()` interpretiert feste Textformate mit Sekunden oder Millisekunden.
- `KW()` nutzt die aktuelle Kultur für Kalenderwochen.
- `DateOfFirstDayInWeek()` berechnet den Beginn einer Kalenderwoche aus Jahr und KW.

## Offene Fragen

- Ob die Parsing-Logik für internationale Formate erweitert werden soll
- Welche Kalenderwochenregel in allen UI- und Reportingbereichen einheitlich gelten muss
- Welche Tests für Randfälle und Kulturabhängigkeit fehlen
