# Feature: Stellt String- und Stream-Erweiterungen bereit

## Beschreibung

Dieses Feature beschreibt die Erweiterungsmethoden für Zeichenketten, Farben und Streams. Die Utilities werden projektübergreifend für Pfadprüfungen, HTML-Konvertierung, Zahlenextraktion, Byte-/Stream-Operationen und Hex-Dumps genutzt.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SStringXtntn.cs`
- `Core.Tests\Core\Extensions\SStringXtntnTests.cs`

## Fachlicher Nutzen

- Pfade können einfach als UNC-, FTP- oder Laufwerkspfad erkannt werden
- Datums- und Zahleninhalte lassen sich aus Texten extrahieren
- HTML-Text kann für Anzeige oder Export kodiert werden
- Streams können in Base64, komprimierte Strings oder Hex-Dumps umgewandelt werden

## Beobachtete Abläufe

- `GetFirstNumber()` sucht die erste numerische Folge in einem Text.
- `AsHTML()` ersetzt Sonderzeichen und deutsche Umlaute durch HTML-Entities.
- `String2Stream()` erzeugt aus Text oder Base64 eine lesbare Stream-Repräsentation.
- `Dump()` formatiert Stream-Inhalte als Hex- und ASCII-Ausgabe.
- Die zugehörigen Tests decken typische Pfad-, Datums- und Streamfälle ab.

## Offene Fragen

- Welche dieser Helfer in produktivem Code am häufigsten verwendet werden
- Ob die Pfadprüfungen für moderne Netzpfade noch erweitert werden müssen
- Welche weiteren Stream-Formate zusätzlich unterstützt werden sollen
