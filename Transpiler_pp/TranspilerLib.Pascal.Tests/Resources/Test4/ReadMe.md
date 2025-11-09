# Test4

Dieser Test prüft:
- Tokenisierung mit Zeilen- und Blockkommentaren (`//`, `(* *)`, `{ }`).
- Hexadezimale Zahlen mit `$`-Präfix (z. B. `$10`, `$0F`).
- Einfache arithmetische Operationen und Zuweisungen.

Dateien:
- `Test4_Source.pas`: Pascal-Quelltext mit Kommentaren und Hexzahlen.
- `Test4_ExpectedTokens.json`: Erwartete Tokenliste für `Tokenize`.
