# Feature: Dokumentiert die Math-Hilfs-Test-Suite für Statistiken und Winkel

## Beschreibung

Dieses Feature beschreibt `SMathTests` als Test-Suite für die mathematischen Core-Hilfen rund um Mittelwerte, Cp/Cpk, Winkel und Grenzen.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Math2\SMathTests.cs`
- `Core\Core\Math2\SMath.cs`
- `Core\Core\Extensions\SListXtntn.cs`
- `Core.Tests\Core\TestData.cs`

## Fachlicher Nutzen

- Numerische Kernfunktionen sind breit datengetrieben abgesichert
- Statistik- und Winkeloperationen bleiben reproduzierbar
- Die Suite zeigt, welche mathematischen Randfälle bereits berücksichtigt werden

## Beobachtete Testinhalte

- Arithmetic-Average-Varianten werden für Listen, Arrays und Queues geprüft.
- `Cp(...)`, `CpK(...)`, `Deg2Rad(...)`, `MinMax(...)`, `DeltaAngle(...)` und `IsBetween(...)` sind umfangreich mit Datenreihen belegt.
- Viele Grenzwerte und Sonderfälle wie `NaN`, `Infinity` und negative Werte werden explizit getestet.

## Offene Fragen

- Ob weitere stabilitätskritische Zahlenfälle ergänzt werden müssen
- Ob die Testdaten langfristig in gemeinsamere Datensammlungen ausgelagert werden sollten
