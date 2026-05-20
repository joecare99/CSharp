# Feature: Überblick über Objektlisten und verzögerte Bool-Komponenten

## Beschreibung

Dieses Feature fasst zwei kleine Core-Komponenten zusammen: `CListOfObjects` als generische Objektlisten-Hilfe und `CDelayedBool` als zeitverzögerte Bool-Komponente.

## Sichtbare technische Bausteine

- `Core\Core\Components\CListOfObjects.cs`
- `Core\Core\Components\CDelayedBool.cs`
- `Core\Core\Extensions\SAsNumericXtntn.cs`

## Fachlicher Nutzen

- Kleinere heterogene Wertegruppen lassen sich in einer Liste zusammenfassen
- Werte können typisiert aus der Liste gelesen werden
- Verzögerte Zustandswechsel können an Zeitbedingungen gekoppelt werden

## Beobachtete Abläufe

- `CListOfObjects` bietet Indexzugriffe auf verschiedene Basistypen und Lookup-Hilfen per Wert.
- `CDelayedBool` speichert Defaultwert, aktuellen Wert und einen Validierungszeitpunkt.
- Beide Komponenten sind pragmatische Legacy-Hilfen für einfache Domänenlogik.

## Offene Fragen

- Ob diese Komponenten in getrennte Fachmodule oder moderne Value-Objekte überführt werden sollten
- Ob die derzeitigen Default- und Indexregeln noch vollständig zu den Fachanforderungen passen
