# Feature: Aktualisiert und ordnet Systemwerte zentral per Bulk-Refresh

## Beschreibung

Dieses Feature beschreibt die zentrale Refresh-Logik rund um `CSystemValues`. Die Registry lädt Werte aus der Datenbank, verfolgt RowVersions und ordnet aktualisierte Werte wieder in die interne Struktur ein.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CSystemValues.cs`
- `Core\Core\System\Values\CSystemValueExt.cs`
- `Core\Core\CommSystem\SSystemValuesHelpers.cs`
- `Core\Core\SQL\CSQLQuery.cs`

## Fachlicher Nutzen

- Die Systemwert-Registry bleibt synchron mit der Datenbank
- Aktualisierte Datensätze werden schnell erkannt
- Ein interner Index erlaubt das Wiederfinden von Werten über UIDs
- Aktualisierungen können an Telegramm- und Kommunikationslogik weitergereicht werden

## Beobachtete Abläufe

- `ReadFirst()` baut die Erstbefüllung der Registry auf.
- `Read()` liest inkrementelle Änderungen anhand der RowVersion.
- `GetSystemValue(...)` sucht per UID in der Indexliste.
- `AddItems(...)` wird beim Import neuer Werte verwendet.

## Offene Fragen

- Ob die Implementierung robuster auf Inkonsistenzen zwischen Indexliste und Dictionary reagieren sollte
- Ob die Registry-Struktur moderner und typsicherer gestaltet werden sollte
