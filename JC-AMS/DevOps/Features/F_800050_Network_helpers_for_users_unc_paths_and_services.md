# Feature: Bietet Netzwerk-, Benutzer- und UNC-Hilfen

## Beschreibung

Dieses Feature beschreibt `SNetHelpers` als Hilfsschicht für lokale Benutzerabfragen, UNC-Pfad-Checks, Netzlaufwerke und Remote-Service-Prüfungen.

## Sichtbare technische Bausteine

- `Core\Core\SNetHelpers.cs`
- `Core\Core\SProcessHelpers.cs`
- `Core\Core\Extensions\SListXtntn.cs`
- `Core\Core\Logging\SLogging.cs`

## Fachlicher Nutzen

- Benutzer- und Domänenkontexte können per Kommandozeile geprüft werden
- UNC-Verbindungen lassen sich testen, aufbauen und trennen
- Netzlaufwerke können verbindlich verbunden oder getrennt werden
- Remote-Dienste und Rechnerpräsenz lassen sich grob diagnostizieren

## Beobachtete Abläufe

- `UserIsKnown(...)` prüft lokale und Domänenbenutzer via `NET USER`.
- `IsUNCPathAvailable(...)` verwendet `NET USE` als Verfügbarkeitsprüfung.
- `ConnectToUNCPath(...)` und `DisconnectFromUNCPath(...)` steuern UNC-Verbindungen.
- `ComputerIsInNetwork(...)` prüft Netzrechner über `NET VIEW`.

## Offene Fragen

- Ob die Abhängigkeit von externen NET-Kommandos künftig abstrahiert werden sollte
- Ob die Remote-Service-Logik separat dokumentiert oder weiter zerlegt werden sollte
