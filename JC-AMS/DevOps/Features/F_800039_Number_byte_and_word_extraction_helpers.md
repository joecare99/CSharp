# Feature: Bietet numerische Byte- und Word-Hilfen

## Beschreibung

Dieses Feature beschreibt die kleinen Data-Operation-Extensions, mit denen Zahlen in High-/Low-Byte, Word und DWord zerlegt werden können.

## Sichtbare technische Bausteine

- `Core\Core\DataOperations\SDataHelpers.cs`
- `Core.Tests\Core\DataOperations\TDataHelpersTests.cs`

## Fachlicher Nutzen

- Binäre Protokolle und Feldgerätewerte können einfach in Teilwerte zerlegt werden
- Byte- und Wortoperationen bleiben zentral und wiederverwendbar
- Die Umrechnung ist gut über Tests abgesichert

## Beobachtete Abläufe

- `HiByte()` und `LoByte()` arbeiten auf `short` und `ushort`.
- `HiWord()` und `LoWord()` arbeiten auf `int` und `uint`.
- `HiDWord()` und `LoDWord()` arbeiten auf `long` und `ulong`.
- Die Tests decken typische Grenzwerte und Kombinationen systematisch ab.

## Offene Fragen

- Ob die Hilfen perspektivisch in eine allgemeinere Numeric-Utility-Klasse überführt werden sollen
- Ob weitere Bit- und Teilwertoperationen noch gebraucht werden
