# Feature: Bildet Systemwerte für Telegramm- und Datenbankprojektionen ab

## Beschreibung

Dieses Feature beschreibt die erweiterte Systemwertklasse, die Core-Systemwerte auf Telegramm-, Datenbank- und SQL-nahe Repräsentationen abbildet.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CSystemValueExt.cs`
- `Core\Core\System\Values\CSystemValue.cs`
- `Core\Core\System\Values\CSystemValueDefExt.cs`
- `Core\Core\CommSystem\Telegram\CTelegramDef.cs`
- `Core\Core\SQL\CSQLQuery.cs`

## Fachlicher Nutzen

- Systemwerte können aus der Datenbank geladen werden
- Telegrammwerte werden typisiert zur passenden Zielrepräsentation konvertiert
- SQL-Schreibdaten können direkt aus Systemwerten erzeugt werden
- Identitäten und Beschreibungen bleiben über Station/Substation/Index nachvollziehbar

## Beobachtete Abläufe

- Die Klasse hält RowVersion, UID, Telegrammdefinitionen und Datenbank-Schlüssel.
- `TelVal` passt den Wert anhand des Ziel-Datentyps an.
- `DBVal` erzeugt bzw. interpretiert Datenbankdarstellungen.
- `Insert()`, `Exist()` und die Konstruktoren arbeiten mit SQL-Abfragen und Kontextdaten.
- Eine UID wird aus ValueDef-, Stations- und Indexdaten berechnet.

## Offene Fragen

- Ob die SQL-Zugriffe modernisiert oder abstrahiert werden sollen
- Wie die fehleranfälligen Sonderfälle rund um Datumswerte und Column-Namen künftig behandelt werden
