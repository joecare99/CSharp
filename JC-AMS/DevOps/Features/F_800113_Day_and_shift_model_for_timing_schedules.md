# Feature: Modelliert Tage und Schichten für Zeitpläne

## Beschreibung

Dieses Feature beschreibt `CDay` als kleines Modell für einen Wochentag mit zugeordneten Schichten.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CDay.cs`
- `Core\Core\Timing\CShift.cs`

## Fachlicher Nutzen

- Zeitpläne können nach Wochentagen gruppiert werden
- Schichten lassen sich pro Tag sammeln und verwalten
- Die Klasse bildet eine einfache Grundstruktur für Schichtmodelle

## Beobachtete Abläufe

- Der Konstruktor speichert den Wochentag.
- Eine Liste von `CShift`-Objekten wird direkt angelegt.
- Weitere Logik ist in der Datei noch nicht sichtbar, was den Modellcharakter unterstreicht.

## Offene Fragen

- Ob weitere Zeitplanlogik in diesem Modell ergänzt werden soll
- Ob der Zugriff auf Felder künftig stärker gekapselt werden sollte
