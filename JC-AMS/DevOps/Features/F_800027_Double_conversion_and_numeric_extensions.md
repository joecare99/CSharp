# Feature: Konvertiert Werte in Double und weitere numerische Typen

## Beschreibung

Dieses Feature beschreibt die numerischen Erweiterungen für Double- und verwandte Typen. Es nutzt robuste Konvertierungsregeln für verschiedene Eingabeformen und wird durch umfangreiche Tests abgesichert.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsDoubleXtntn.cs`
- `Core.Tests\Core\Extensions\SAsDoubleXtntnTests.cs`
- `Core\Core\Extensions\SAsNumericXtntn.cs`
- `Core.Tests\Core\Extensions\SAsNumericXtntnTests.cs`

## Fachlicher Nutzen

- Zahlenwerte können aus Objekten, Strings und Bytefolgen extrahiert werden
- Numerische Konvertierungen bleiben robust gegen Sonderwerte
- Zeitstempel-, Größen- und Gleitkomma-Konvertierungen sind zentral verfügbar
- Tests dokumentieren Grenzfälle und plattformspezifische Unterschiede

## Beobachtete Abläufe

- `AsDouble()` verarbeitet Zahlen, Strings und Bytearrays.
- `AsFloat()` und die weiteren Helfer bilden ähnliche Konvertierungswege ab.
- `SAsNumericXtntnTests` decken DateTime, Size, String, Zeitstempel und Integer-basierte Konvertierungen ab.
- Die Tests zeigen auch den Umgang mit `NaN`, `Infinity` und extremen Grenzwerten.

## Offene Fragen

- Welche numerischen Konvertierungen zusätzlich im Produktivcode genutzt werden
- Ob die Zielgenauigkeit plattformabhängig normiert werden sollte
- Welche Spezialfälle in den Tests noch fehlen
