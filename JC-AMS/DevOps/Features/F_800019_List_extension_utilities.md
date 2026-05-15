# Feature: Stellt Listen-Erweiterungen bereit

## Beschreibung

Dieses Feature beschreibt die Erweiterungsmethoden für Listen, insbesondere fallunabhängige Suchlogik auf Basis von Textinhalten.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SListXtntn.cs`
- `Core.Tests\Core\Extensions\SListXtntnTests.cs`

## Fachlicher Nutzen

- Listen von Beschreibungen können robust und case-insensitive durchsucht werden
- Teiltreffer in zusammengesetzten Texten werden berücksichtigt
- Die Suche kann gegen `null`-Werte abgesichert werden
- Tests dokumentieren die erwarteten Suchregeln

## Beobachtete Abläufe

- `ContainsCI()` prüft exakte Treffer, Wortanfänge und enthaltene Wortfolgen.
- Leere Suchbegriffe oder `null`-Listen liefern `false`.
- Fehler in der Erweiterung werden protokolliert.
- Die Tests zeigen die aktuelle gewünschte Semantik für Sonderfälle und Teiltreffer.

## Offene Fragen

- Ob die Wortgrenzen-Semantik bewusst so breit bleiben soll
- Ob die Methode auch für andere Sammlungen als `List<string>` relevant ist
- Ob weitere Listenhilfen im selben Stil ergänzt werden sollen
