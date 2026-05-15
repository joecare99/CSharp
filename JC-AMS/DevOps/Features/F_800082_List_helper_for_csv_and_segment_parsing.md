# Feature: Bietet List-Hilfen für CSV- und Segment-Verarbeitung

## Beschreibung

Dieses Feature beschreibt `SListHelper` als Utility für die Umwandlung zwischen Listen und CSV-/Segmentdarstellungen.

## Sichtbare technische Bausteine

- `Core\Core\SListHelper.cs`
- `Core\Core\Components\CListOfObjects.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Listen können schnell als CSV serialisiert werden
- Zeichenfolgen können in Integer- oder Objektlisten zerlegt werden
- Einfach getrennte Segmente lassen sich normalisieren

## Beobachtete Abläufe

- `GetCSVFromList(...)` joiniert Listeneinträge mit einem Separator.
- `GetCSVFromVariableSeparatedText(...)` ersetzt typische Trennzeichen durch Semikolons.
- `GetListOfIntFromCSV(...)` und `GetListOfStringFromCSV(...)` erzeugen typisierte Listen.
- `GetListOfListOf3IntFromCSV(...)` parst Dreiergruppen in `CListOfObjects`.

## Offene Fragen

- Ob die Parsingregeln für Variablen-Trenntexte weiter vereinheitlicht werden sollten
- Ob leere oder fehlerhafte Segmente noch strenger behandelt werden müssen
