# Feature: Dokumentiert die Integer-Konvertierungs-Test-Suite für tolerantes Parsing

## Beschreibung

Dieses Feature beschreibt `SAsIntXtntnTests` als Test-Suite für die integerbasierte Konvertierungslogik.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SAsIntXtntnTests.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Integer-Konvertierungen bleiben für Objekte und Strings nachvollziehbar
- Grenzwerte und Fehlerfälle sind dokumentiert
- Die Regex-Logik zur Erkennung ganzer Zahlen wird sichtbar überprüft

## Beobachtete Testinhalte

- `AsInt32(...)` und `AsInt64(...)` werden für Objekte und Strings geprüft.
- `IsInt(...)` wird gegen typische, grenzwertige und fehlerhafte Strings getestet.
- Die Suite enthält Tests für `DBNull`, `double.NaN`, `Infinity` und sehr große Werte.
- Einige Datensätze sind kommentiert und markieren offene Interpretationsfragen.

## Offene Fragen

- Ob die Fehlerbehandlung auf `0` für alle ungültigen Werte fachlich gewünscht ist
- Ob die String- und Objektpfade semantisch vollständig identisch bleiben sollen
