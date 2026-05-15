# Feature: Dokumentiert die List-Extension-Test-Suite für case-insensitive Matching

## Beschreibung

Dieses Feature beschreibt `SListXtntnTests` als Test-Suite für die case-insensitive Listen-Suche.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SListXtntnTests.cs`
- `Core\Core\Extensions\SListXtntn.cs`

## Fachlicher Nutzen

- Listen-Matching-Verhalten ist direkt testbar
- Groß-/Kleinschreibung und Teilwortlogik werden abgesichert
- Null- und Leerliste-Fälle sind erfasst

## Beobachtete Testinhalte

- `ContainsCITest` und `ContainsCITest2` prüfen denselben Kern sowohl instanz- als auch statisch aufgerufen.
- Die Datenmenge enthält Nullfälle, Einzelwerte und Mehrworttreffer.
- Ein Testfall ist mit einem Kommentar als fraglich markiert und zeigt so mögliche Spezifikationslücken.

## Offene Fragen

- Ob die Teilwortlogik exakt so beabsichtigt ist oder noch präzisiert werden sollte
- Ob die Tests um weitere Grenzfälle erweitert werden sollen
