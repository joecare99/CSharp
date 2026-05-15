# Feature: Vermittelt Kommunikation zwischen Service und Clients

## Beschreibung

Dieses Feature beschreibt die Kommunikationsschicht zwischen Dienst, Clients und Telegrammverarbeitung. Die Solution nutzt dazu eine Service-Kommunikation, Client-Verwaltung und telegrammbasierte Verarbeitung.

## Sichtbare technische Bausteine

- `Communication\Communication.Service\CCommClient.cs`: Clientverarbeitung, Telegrammhandling und Fehlerreaktionen
- `Communication\Communication.Service\CControlServiceCommunication.cs`: zentrale Steuerung des Kommunikationsdienstes
- `Core\Core\CommSystem`: Telegramme, Kommunikationspartner, Mailboxen und Protokolle
- `Core\Core\Communication`: Netzwerk- und Kommunikationshilfen

## Fachlicher Nutzen

- Kommunikationspartner können sauber verwaltet werden
- Eingehende Telegramme werden nach Service- und Datenbanklogik aufgeteilt
- Wiederverbindung und Fehlerfälle lassen sich getrennt behandeln
- Service- und Fachlogik bleiben entkoppelt

## Offene Fragen

- Welche Telegrammtypen zentral verarbeitet werden
- Welche Nachrichten an Datenbank, Service oder Handler weitergereicht werden
- Wie Kommunikationsabbrüche protokolliert und wiederhergestellt werden
