# Feature: Verbindet Instanzen über Named Pipes oder Fallback-Verbindungen

## Beschreibung

Dieses Feature beschreibt `CIPC` als Kommunikations-Manager, der Named Pipes zwischen Instanzen aufbaut und je nach Betriebsart auf Datenbank- oder Telegrammwege ausweichen kann.

## Sichtbare technische Bausteine

- `Core\Core\Components\CIPC.cs`
- `Core\Core\Communication\NamedPipes\CNamedPipe.cs`
- `Core\Core\Communication\NamedPipes\NamedPipeReadEventHandler.cs`
- `Core\Core\System\SConfiguration.cs`
- `Core\Core\System\SJCAMS.cs`

## Fachlicher Nutzen

- Instanzen können über Named Pipes bidirektional verbunden werden
- Manager-, CommService- und ProcessService-Szenarien werden getrennt behandelt
- Fallback auf Datenbank oder Telegramm bleibt möglich

## Beobachtete Abläufe

- Konstruktoren wählen die Verbindungsart abhängig von Instanztyp und Konfiguration.
- `AddNamedPipe(...)` erstellt Pipe-Objekte und hängt Event-Handler an.
- Die Klasse berücksichtigt Produktionszellen, Copilot- und Master-Beziehungen.
- Mehrere Betriebsmodi setzen unterschiedliche Verbindungsrichtungen für Manager und Client.

## Offene Fragen

- Ob die Verbindungslogik weiter in einzelne Strategien zerlegt werden sollte
- Ob die vielen Instanzprüfungen künftig zentralisiert werden können
