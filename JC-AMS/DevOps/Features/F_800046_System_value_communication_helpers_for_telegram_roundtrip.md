# Feature: Verarbeitet Systemwerte für Telegramm-Roundtrips

## Beschreibung

Dieses Feature beschreibt die Kommunikations-Helfer, die Telegrammfelder in Systemwerte schreiben und daraus wieder auslesen. Die Klasse bündelt dabei Batch-Inserts, Monitoring-Logging und Speziallogik für mehrere Betriebsarten.

## Sichtbare technische Bausteine

- `Core\Core\CommSystem\SSystemValuesHelpers.cs`
- `Core\Core\System\Values\CSystemValues.cs`
- `Core\Core\System\Values\CSystemValueExt.cs`
- `Core\Core\CommSystem\Telegram\CTelegram.cs`
- `Core\Core\CommSystem\Telegram\CTelegramFieldValue.cs`

## Fachlicher Nutzen

- Telegrammwerte werden in Systemwert-Tabellen zurückgeschrieben
- TelTime wird als Spezialwert gepflegt
- Änderungslisten können gebündelt in die Datenbank geschrieben werden
- Monitored-Values lassen sich gezielt protokollieren

## Beobachtete Abläufe

- `SetFromTelegram(...)` und Varianten erzeugen SQL-Insert-Statements für Systemwerte.
- `SetTelValFromTelegram(...)` arbeitet mit der Telegrammprojektion `TelVal`.
- `ReadFirst()` und `Read()` synchronisieren die Registry mit der Datenbank.
- Für bestimmte Storage-Handling-Konstellationen werden zusätzliche Telegramme zum Senden vorgemerkt.

## Offene Fragen

- Ob die enge Abhängigkeit zu AGV- und Storage-Logik weiter entkoppelt werden soll
- Ob die SQL-Batchbildung künftig in eine eigene Persistenzschicht ausgelagert werden sollte
