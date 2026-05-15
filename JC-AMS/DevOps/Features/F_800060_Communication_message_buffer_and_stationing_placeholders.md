# Feature: Enthält einfache Kommunikations-Puffer- und Stationing-Platzhalter

## Beschreibung

Dieses Feature beschreibt kleinere Kommunikationsbausteine wie Nachrichtentypen, einfache Buffer-Objekte und Platzhalterklassen für Stationing-Strukturen.

## Sichtbare technische Bausteine

- `Core\Core\CommSystem\CMsgBuffer.cs`
- `Core\Core\CommSystem\Communicator\CCommunicationStationing.cs`
- `Core\Core\Communication\StateObject.cs`
- `Core\Core\Communication\NamedPipes\CNamedPipe.cs`

## Fachlicher Nutzen

- Telegramme und Empfangsdaten können in kleinen Transportobjekten gehalten werden
- Stationing-Schichten besitzen einen Platzhalter für spätere Erweiterungen
- Socket- und Pipe-Workflows erhalten einfache State-Objekte

## Beobachtete Abläufe

- `CMsgBuffer` speichert Zeit, Richtungsinformation und Telegrammbytes.
- `StateObject` bündelt Socket und Bytebuffer für asynchrone Kommunikation.
- `CCommunicationStationing` ist aktuell nur ein leeres Platzhaltermodell.

## Offene Fragen

- Ob `CCommunicationStationing` künftig eine echte Domänenrolle erhält
- Ob Buffer- und State-Objekte konsolidiert oder getrennt bleiben sollen
