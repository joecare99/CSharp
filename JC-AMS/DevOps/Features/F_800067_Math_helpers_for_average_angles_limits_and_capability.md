# Feature: Bietet mathematische Hilfen für Mittelwerte, Winkel und Fähigkeitskennzahlen

## Beschreibung

Dieses Feature beschreibt `SMath` als allgemeine Math-Hilfsklasse für Durchschnittswerte, Cp/Cpk-Berechnung, Winkelumrechnung und Wertebegrenzung.

## Sichtbare technische Bausteine

- `Core\Core\Math2\SMath.cs`
- `Core.Tests\Core\Math2\SMathTests.cs`
- `Core\Core\Extensions\SListXtntn.cs`

## Fachlicher Nutzen

- Mittelwerte können für Listen, Arrays und Queues berechnet werden
- Prozessfähigkeitskennzahlen Cp und Cpk stehen zentral zur Verfügung
- Winkel lassen sich zwischen Grad und Radiant umrechnen
- Werte können generisch auf einen zulässigen Bereich begrenzt werden

## Beobachtete Abläufe

- `ArithmeticAverage(...)` existiert in mehreren Überladungen für verschiedene Eingabetypen.
- `Cp(...)` und `CpK(...)` behandeln Null-, NaN- und Sonderfälle defensiv.
- `Deg2Rad(...)` liefert Umrechnungen für `float` und `double`.
- `MinMax(...)` und `Limit(...)` kapseln Bereichsbegrenzungen generisch.
- `DeltaAngle(...)` und `DeltaAngleClockwise(...)` berechnen Winkelabweichungen.

## Offene Fragen

- Ob die fachlich sehr unterschiedlichen Methoden künftig thematisch stärker aufgeteilt werden sollten
- Ob die Prozessfähigkeitslogik für alle Sonderfälle noch modernisiert werden sollte
