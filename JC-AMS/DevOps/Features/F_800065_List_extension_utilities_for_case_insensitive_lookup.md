# Feature: Bietet Listen-Erweiterungen für case-insensitive Suchen

## Beschreibung

Dieses Feature beschreibt `SListXtntn` als kleine Erweiterung für case-insensitive String-Suchen in Listen.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SListXtntn.cs`
- `Core.Tests\Core\Extensions\SListXtntnTests.cs`

## Fachlicher Nutzen

- Benutzer- und Domänenlisten können flexibel auf Einträge geprüft werden
- Die Suche ist tolerant gegenüber Groß-/Kleinschreibung
- Teilwörter am Anfang oder in der Mitte von Einträgen werden berücksichtigt

## Beobachtete Abläufe

- `ContainsCI(...)` prüft eine Liste von Strings gegen einen Suchbegriff.
- Leerwerte und `null` werden defensiv abgefangen.
- Fehler werden geloggt, statt die Anwendung zu unterbrechen.
- Die Tests prüfen sowohl den Instanz- als auch den statischen Aufrufpfad.

## Offene Fragen

- Ob die Suche im Falle von Sonderzeichen und Wortgrenzen noch präzisiert werden sollte
- Ob der Namensraum der Extension-Klasse einheitlicher benannt werden soll
