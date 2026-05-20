# Feature: Dokumentiert die numerische Konvertierungs-Test-Suite für Datum, Größe und Zeitstempel

## Beschreibung

Dieses Feature beschreibt `SAsNumericXtntnTests` als umfassende Test-Suite für die numerischen Konvertierungs-Extensions.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SAsNumericXtntnTests.cs`
- `Core\Core\Extensions\SAsNumericXtntn.cs`
- `System.Drawing.Size`

## Fachlicher Nutzen

- Die vielseitigen Objekt-zu-Typ-Konvertierungen sind breit abgesichert
- DateTime-, Size-, Timestamp- und Unsigned-Konvertierungen bleiben reproduzierbar
- Die Suite macht kulturabhängige und binäre Interpretationen transparent

## Beobachtete Testinhalte

- `AsDateTime(...)`, `AsSize(...)` und `AsString(...)` werden für typische und fehlerhafte Eingaben geprüft.
- `AsTimeStampS(...)` und `AsTimeStampL(...)` testen Hex- und Byte-Array-Interpretationen.
- `AsUInt64(...)` und `AsUInt32(...)` werden über viele Grenzfälle verifiziert.
- Die Suite enthält kulturabhängige Stringwerte und Plattformunterschiede.

## Offene Fragen

- Ob die sehr breite Konvertierungslogik in Zukunft in kleinere Spezial-Utilities zerlegt werden sollte
- Ob noch weitere kulturabhängige Testfälle ergänzt werden müssen
