# Feature: Bietet String-Erweiterungen für Pfade, HTML und Streams

## Beschreibung

Dieses Feature beschreibt `SStringXtntn` als umfangreiche String-Utility-Klasse für Pfadklassifikation, HTML-Encoding, numerische Extraktion und Stream-Hilfen.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SStringXtntn.cs`
- `Core.Tests\Core\Extensions\SStringXtntnTests.cs`

## Fachlicher Nutzen

- Pfade können als UNC-, FTP- oder Laufwerkspfade erkannt werden
- Text kann HTML-kompatibel kodiert werden
- Zahlen und Datumswerte lassen sich aus Texten extrahieren oder prüfen
- Streams können in Text-, Base64- oder Dump-Formate umgewandelt werden

## Beobachtete Abläufe

- `GetEncapsulated(...)` extrahiert Text zwischen zwei Markern.
- `IsUNCPath(...)`, `IsFTPPath(...)` und `IsDrivePath(...)` klassifizieren Pfade.
- `IsDateTime(...)` und `IsDouble(...)` prüfen Text auf numerische bzw. Datumsformate.
- `AsHTML(...)` kodiert Sonderzeichen und `Color`-Werte.
- `String2Stream(...)`, `AsCompString(...)`, `AsBase64String(...)` und `Dump(...)` bilden Stream-Hilfen.

## Offene Fragen

- Ob die vielen String-Helfer künftig in thematisch getrennte Hilfsklassen aufgeteilt werden sollen
- Ob die Pfad- und Formatprüfungen robuster gegen ungültige Indizes gemacht werden sollen
