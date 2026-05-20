# Feature: Modelliert Feiertage und deren Gültigkeit mit Persistenz

## Beschreibung

Dieses Feature beschreibt `CHolidaysOfYear` und `CHoliday` als Kalender- und Feiertagsmodell mit Persistenz der Gültigkeit.

## Sichtbare technische Bausteine

- `Core\Core\Timing\Holidays\CHolidaysOfYear.cs`
- `Core\Core\Timing\Holidays\CHoliday.cs`
- `Core\Core\SVariableHandling.cs`
- `Core\Core\Extensions\SAsBoolXtntn.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Feiertage können pro Jahr zentral berechnet und verwaltet werden
- Feste und variable Feiertage werden getrennt modelliert
- Die Gültigkeit einzelner Feiertage kann gespeichert und wieder geladen werden

## Beobachtete Abläufe

- `GetInstance(...)` liefert eine jahresspezifische Singleton-Instanz.
- `Set(...)` erzeugt feste und bewegliche Feiertage.
- `GetOsterSonntag()` berechnet das Osterdatum algorithmisch.
- `SaveValidity()` und `LoadValidity()` persistieren die Aktivierung der Feiertage.
- `CHoliday` speichert ID, Fixstatus, Datum, Name und Vergleichslogik.

## Offene Fragen

- Ob die Feiertagsliste für weitere Länder oder Regionen erweitert werden soll
- Ob die Persistenz der Gültigkeit noch eine modernere Ablageform erhalten sollte
