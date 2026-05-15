# Feature: Hält gleitende und einfache Durchschnittswerte in kleinen Value-Objekten

## Beschreibung

Dieses Feature beschreibt `CAverage` und `CArithmeticAverage` als kleine Wertobjekte für laufende Durchschnittsbildung und Statistikpuffer.

## Sichtbare technische Bausteine

- `Core\Core\Math2\CAverage.cs`
- `Core\Core\Math2\CArithmeticAverage.cs`
- `Core\Core\Communication\CPing.cs`
- `Core.Tests\Core\Math2\SMathTests.cs`

## Fachlicher Nutzen

- Durchschnittswerte können inkrementell berechnet werden
- Rolling-Window-Statistiken bleiben einfach und threadbewusst
- Kommunikations- und Messfunktionen können Laufzeiten auswerten

## Beobachtete Abläufe

- `CAverage` akkumuliert Werte über einen einfachen Zähler und Mittelwert.
- `CArithmeticAverage` speichert eine Liste von Werten und begrenzt die Größe optional.
- `ArithmeticAverage`, `MinValue`, `MaxValue` und `Count` werden aus dem Puffer abgeleitet.
- `Dispose()` leert den internen Wertepuffer.

## Offene Fragen

- Ob die beiden Durchschnittsvarianten konzeptionell zusammengelegt werden sollten
- Ob die Thread-Synchronisation für alle Einsatzszenarien ausreichend ist
