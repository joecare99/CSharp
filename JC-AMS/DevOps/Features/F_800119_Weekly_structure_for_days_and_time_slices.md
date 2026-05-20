# Feature: Bietet eine Wochenstruktur aus Tagen

## Beschreibung

Dieses Feature beschreibt `CWeek` als einfache Wochenstruktur mit sieben `CDay`-Einträgen.

## Sichtbare technische Bausteine

- `Core\Core\Timing\CWeek.cs`
- `Core\Core\Timing\CDay.cs`

## Fachlicher Nutzen

- Wochenbasierte Planungsmodelle erhalten eine feste Grundstruktur
- Jeder Tag der Woche kann separat mit Schichten belegt werden
- Die Klasse ist ein einfacher Organisationsanker für Zeitplanung

## Beobachtete Abläufe

- Der Konstruktor erzeugt sieben Tagesobjekte.
- Jeder Index wird mit dem passenden `DayOfWeek` belegt.

## Offene Fragen

- Ob weitere Hilfsfunktionen zur Navigation zwischen Wochentagen ergänzt werden sollten
- Ob die Klasse künftig stärker gekapselt werden sollte
