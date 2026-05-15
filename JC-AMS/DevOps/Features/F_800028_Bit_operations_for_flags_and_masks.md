# Feature: Stellt Bit-Operationen für Flags und Masken bereit

## Beschreibung

Dieses Feature beschreibt die Bit-Hilfen für Flag-Prüfung und Bit-Setzen in verschiedenen Integer-Typen. Es wird für Zustände, Masken und kleine Steuerlogik genutzt.

## Sichtbare technische Bausteine

- `Core\Core\DataOperations\SBitOperations.cs`
- `Core.Tests\Core\DataOperations\TBitOperationsTests.cs`

## Fachlicher Nutzen

- Bitmasken können typübergreifend geprüft werden
- Zustände lassen sich in Integerwerten setzen und löschen
- Die Logik bleibt für `int`, `long`, `uint` und `ulong` konsistent
- Tests dokumentieren erwartete Grenzfälle bis zum höchsten Bit

## Beobachtete Abläufe

- `IsBitSet()` prüft einzelne Bits in verschiedenen Ganzzahltypen.
- `SetBit()` setzt oder löscht Bits auch per By-Ref-Variante.
- Die Tests decken positive, negative und Grenzwerte wie Bit 31 und 63 ab.

## Offene Fragen

- Welche Flag-Felder im restlichen Code diese Helfer nutzen
- Ob weitere Komfortmethoden für Bitmasken benötigt werden
- Ob Fehlerfälle bei ungültigen Bitindizes einheitlich behandelt werden sollen
