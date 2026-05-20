# Feature: Verbindet Produktivitätszeiträume und Feiertage in einem Zeitplaner

## Beschreibung

Dieses Feature beschreibt `CTimeScheduler` als Zusammenspiel aus Produktivitätszeiten und Feiertagskalendern.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CTimeScheduler.cs`
- `Core\Core\Timing\CTimespansOfProductivity.cs`
- `Core\Core\Timing\Holidays\CHolidaysOfYear.cs`

## Fachlicher Nutzen

- Zeitplanung kann produktive Zeiten und Feiertage gemeinsam berücksichtigen
- Ein Startobjekt bündelt die beiden wichtigen Zeitachsen
- Die Klasse liefert eine zentrale Einstiegshilfe für Schicht- und Kalenderlogik

## Beobachtete Abläufe

- Der Konstruktor lädt Produktivitätszeiträume sofort aus der Datenquelle.
- Für das aktuelle Jahr wird ein Feiertagskalender erzeugt.
- Beide Bereiche stehen als Eigenschaften zur Verfügung.

## Offene Fragen

- Ob die automatische Initialisierung in allen Kontexten erwünscht ist
- Ob Fehlersituationen beim Laden der Produktivitätszeiten besser sichtbar gemacht werden sollten
