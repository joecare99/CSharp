# Feature: Modelliert Wochen, Tage und Schichten für Schedules

## Beschreibung

Dieses Feature beschreibt `CWeek`, `CDay` und `CShift` als einfache Scheduling-Modelle für Wochentage und Schichtzeiten.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CWeek.cs`
- `Core\Core\Timing\CDay.cs`
- `Core\Core\Timing\CShift.cs`

## Fachlicher Nutzen

- Wochentage können mit Schichten strukturiert werden
- Einfache Produktions- oder Anlagenpläne lassen sich zeitlich abbilden
- Die Klassen bilden eine kompakte Planungsgrundlage

## Beobachtete Abläufe

- `CWeek` erzeugt standardmäßig sieben `CDay`-Objekte.
- `CDay` hält einen Wochentag und eine Liste von Schichten.
- `CShift` speichert Wochentag, Schichtnummer, Beginn und Ende.

## Offene Fragen

- Ob diese einfachen Modelle durch komplexere Planungsobjekte ersetzt werden sollten
- Ob die derzeitige Feldstruktur zugunsten von Properties modernisiert werden sollte
