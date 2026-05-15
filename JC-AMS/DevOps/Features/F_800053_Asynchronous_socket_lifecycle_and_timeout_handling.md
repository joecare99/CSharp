# Feature: Verarbeitet asynchrone Socket-Lebenszyklen und Timeouts

## Beschreibung

Dieses Feature beschreibt `CAsynchronousSocket` als Kommunikationsbasis für asynchrone TCP-Sockets mit Verbindungsaufbau, Trennung und Timeout-Verwaltung.

## Sichtbare technische Bausteine

- `Core\Core\Communication\CAsynchronousSocket.cs`
- `Core\Core\CommSystem\Telegram\CTelegram.cs`
- `Core\Core\System\SConfiguration.cs`

## Fachlicher Nutzen

- TCP-Verbindungen können asynchron aufgebaut und getrennt werden
- Timeouts und Puffergrößen sind zentral konfigurierbar
- Socket-Zustände lassen sich für Kommunikationskanäle wiederverwenden

## Beobachtete Abläufe

- Der Konstruktor übernimmt Socket-Parameter und initialisiert Timeout- und Pufferwerte.
- `Connect()` startet den asynchronen Verbindungsaufbau.
- `OnConnect()` setzt Timer zurück und markiert die Verbindung als hergestellt.
- `Disconnect()` stößt die asynchrone Trennung an und ruft sonst `Close()` auf.

## Offene Fragen

- Ob die Socket-Basis künftig in eine klarere Kommunikationsabstraktion überführt werden soll
- Ob die manuellen Timeout-Felder weiterhin in dieser Form benötigt werden
