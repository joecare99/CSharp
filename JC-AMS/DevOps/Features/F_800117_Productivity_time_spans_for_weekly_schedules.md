# Feature: Verwaltet wöchentliche Produktivitätszeitspannen

## Beschreibung

Dieses Feature beschreibt `CTimespansOfProductivity` als Sammlung produktiver Zeitfenster pro Woche samt inverser Ableitung.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CTimespansOfProductivity.cs`
- `Core\Core\Timing\CTimeSpanInWeek.cs`
- `Core\Core\SQL\CSQLQuery.cs`
- `Core\Core\System\SConfiguration.cs`

## Fachlicher Nutzen

- Produktive Zeitfenster können aus der Datenbank geladen und gespeichert werden
- Inverse Zeiträume lassen sich aus den Produktivzeiten ableiten
- Die Klasse ist Grundlage für Wochen- und Maschinenzeitplanung

## Beobachtete Abläufe

- `Load()` liest Zeitspannen aus `ProductionTime` und füllt die Sammlung.
- `Save()` schreibt die aktuellen Zeitspannen zurück in die Datenbank.
- `SetInverseItems()` ermittelt nicht-produktive Zwischenräume.
- `GetNearesProductiveTime(...)` sucht das nächste produktive Zeitfenster zu einem Testzeitpunkt.

## Offene Fragen

- Ob die Berechnung des nächsten produktiven Zeitpunkts noch vollständig korrekt ist
- Ob die inverse Zeitspanne bei Randfällen oder Tageswechseln weiter abgesichert werden sollte
