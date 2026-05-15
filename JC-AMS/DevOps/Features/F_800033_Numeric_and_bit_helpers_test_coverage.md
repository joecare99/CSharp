# Feature: Prüft numerische und Bit-Hilfen im Core

## Beschreibung

Dieses Feature beschreibt die Testabdeckung für numerische Konvertierungen, Bit-Operationen und allgemeine Hilfsfunktionen im Core. Die Tests zeigen, wie robust die Hilfsmethoden mit Grenzwerten und Sonderfällen umgehen.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SAsNumericXtntnTests.cs`
- `Core.Tests\Core\Extensions\SAsDoubleXtntnTests.cs`
- `Core.Tests\Core\DataOperations\TBitOperationsTests.cs`
- `Core.Tests\Core\Math2\SMath2Tests.cs`
- zugehörige Core-Utilities wie `SAsNumericXtntn`, `SAsDoubleXtntn`, `SBitOperations`, `SMath2`

## Fachlicher Nutzen

- Typkonvertierungen bleiben für Zahlen, Arrays und Sonderwerte nachvollziehbar
- Bitmasken lassen sich konsistent prüfen und setzen
- Geometrische und numerische Hilfsfunktionen werden testbar gemacht
- Plattform- und Grenzfallunterschiede werden sichtbar

## Beobachtete Testinhalte

- `SAsNumericXtntnTests` prüfen DateTime-, Size-, String-, Timestamp- und Numerik-Konvertierungen.
- `SAsDoubleXtntnTests` decken Double- und Float-Konvertierungen aus unterschiedlichen Eingabetypen ab.
- `TBitOperationsTests` validieren einzelne Bits in `int`, `long`, `uint` und `ulong`.
- `SMath2Tests` enthalten Tests für Punkt-/Vektor-Konvertierungen und mehrere noch offene Geometriefälle.

## Offene Fragen

- Ob die numerischen Tests vollständig genug für alle Zielplattformen sind
- Welche Geometriefunktionen in `SMath2` noch weiter dokumentiert oder getestet werden sollen
- Ob Grenzfälle bei `NaN`, `Infinity` und Bytefolgen noch systematischer erfasst werden müssen
