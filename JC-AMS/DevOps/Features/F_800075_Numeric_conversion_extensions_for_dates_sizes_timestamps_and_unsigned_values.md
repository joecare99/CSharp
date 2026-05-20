# Feature: Bietet numerische Konvertierungs-Extensions für Datums-, Größen- und Timestamp-Werte

## Beschreibung

Dieses Feature beschreibt `SAsNumericXtntn` als zentrale Konvertierungsschicht für unterschiedliche Objektformen in `DateTime`, `Size`, `string`, Zeitstempel und unsigned Zahlen.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsNumericXtntn.cs`
- `Core.Tests\Core\Extensions\SAsNumericXtntnTests.cs`
- `System.Drawing.Size`
- `System.Globalization`

## Fachlicher Nutzen

- Heterogene Werte können konsistent umgewandelt werden
- DateTime-, Size- und String-Konvertierungen sind zentral verfügbar
- Zeitstempel lassen sich in numerische und hexadezimale Formen übertragen
- Unsigned Werte können aus verschiedenen Quellen robust extrahiert werden

## Beobachtete Abläufe

- `AsDateTime(...)` behandelt `DBNull`, `DateTime` und stringbasierte Eingaben.
- `AsSize(...)` akzeptiert sowohl `int[]` als auch semikolongetrennte Strings.
- `AsString(...)` normalisiert `null` und `DBNull` zu leeren Strings.
- `AsTimeStampS(...)` und `AsTimeStampL(...)` konvertieren in Hex-/Long-Formate.
- `AsUInt64(...)` und `AsUInt32(...)` behandeln Hexstrings, Arrays und numerische Typen.

## Offene Fragen

- Ob die Sonderfälle bei Datums- und Timestamp-Konvertierung weiter vereinheitlicht werden sollten
- Ob zusätzliche Tests für kulturelle Formate und Grenzwerte notwendig sind
