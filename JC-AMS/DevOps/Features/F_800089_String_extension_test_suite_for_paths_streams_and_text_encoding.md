# Feature: Dokumentiert die String-Extension-Test-Suite für Pfade, Streams und Textkodierung

## Beschreibung

Dieses Feature beschreibt `SStringXtntnTests` als umfangreiche Test-Suite für die String-Utilities aus dem Core-Bereich.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SStringXtntnTests.cs`
- `Core\Core\Extensions\SStringXtntn.cs`
- `Core.Tests\Core\TestData.cs`

## Fachlicher Nutzen

- Pfad- und Text-Utilities bleiben reproduzierbar abgesichert
- Stream-, Base64- und GZip-Verhalten ist dokumentiert
- Numerische Extraktion und HTML-Kodierung werden sichtbar geprüft

## Beobachtete Testinhalte

- UNC-, FTP- und Datums-/Zahlenerkennung werden mit Datenreihen getestet.
- HTML-Encoding deckt Sonderzeichen, Umlaute und Color-Formatierung ab.
- `GetFirstNumber(...)`, `CharCnt(...)`, `Dump(...)`, `String2Stream(...)`, `AsCompString(...)` und `AsBase64String(...)` sind breit belegt.
- Die Suite nutzt sowohl normalisierte als auch kommentierte Randfälle.

## Offene Fragen

- Ob die String-Utility-Klasse künftig stärker in kleinere thematische Bereiche geteilt werden sollte
- Ob die vielen kommentierten Randfälle in dauerhafte Spezifikation oder technische Schulden überführt werden sollen
