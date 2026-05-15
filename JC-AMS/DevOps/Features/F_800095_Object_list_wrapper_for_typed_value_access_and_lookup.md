# Feature: Kapselt Object-Listen für typisierte Wertzugriffe und Lookup-Hilfen

## Beschreibung

Dieses Feature beschreibt `CListOfObjects` als kleine Objektlisten-Hilfsklasse mit typisierten Zugriffsmethoden und Lookup-Funktionen.

## Sichtbare technische Bausteine

- `Core\Core\Components\CListOfObjects.cs`
- `Core\Core\Extensions\SAsNumericXtntn.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Heterogene Werte können kompakt zusammengefasst werden
- Indexbasierte, typisierte Zugriffe erleichtern einfache Datencontainer
- Lookup-Methoden unterstützen Beschreibungszuordnungen

## Beobachtete Abläufe

- `DisplayMember` und `ValueMember` greifen typisiert auf Positionen zu.
- Zugriffsmethoden wie `DateTime(...)`, `Int(...)`, `Double(...)`, `Bool(...)`, `String(...)` und `Object(...)` liefern Werte aus der internen Liste.
- `Set(...)` erweitert die Liste bei Bedarf.
- `DescForValueInAL(...)` und `DescForValueInALi(...)` suchen Einträge in Listen von Objektlisten.

## Offene Fragen

- Ob diese generische Objektliste modernere Typisierung ersetzen sollte
- Ob die Indexsemantik in allen Aufrufern eindeutig genug ist
