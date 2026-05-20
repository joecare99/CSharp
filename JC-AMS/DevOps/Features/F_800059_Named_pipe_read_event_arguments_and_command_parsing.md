# Feature: Parst Named-Pipe-Eingaben in Befehle und Statements

## Beschreibung

Dieses Feature beschreibt `CNamedPipeReadEventArgs` und den zugehörigen Delegate-Typ. Die Klasse zerlegt Pipe-Eingaben in Statement, Befehl und Argumente.

## Sichtbare technische Bausteine

- `Core\Core\Communication\NamedPipes\CNamedPipeReadEventArgs.cs`
- `Core\Core\Communication\NamedPipes\NamedPipeReadEventHandler.cs`
- `Core\Core\CommandSystem\CommandStatement.cs`
- `Core\Core\CommandSystem\CCommand.cs`

## Fachlicher Nutzen

- Pipe-Nachrichten werden in strukturierte Command-Daten überführt
- Ping-Sonderfälle und normale Statements werden getrennt behandelt
- Der Event-Mechanismus erleichtert die Entkopplung von Pipe und Verarbeitung

## Beobachtete Abläufe

- `Input` wird direkt gespeichert und analysiert.
- Ein `Ping`-Präfix führt zu `CommandStatement.Ping`.
- Längere Eingaben werden über feste Positionsfelder in Statement, Command-String und Argumente zerlegt.
- `Command` erzeugt ein `CCommand`-Objekt aus dem Textbefehl.

## Offene Fragen

- Ob die feste Positionslogik für das Parsing noch aktuell ist
- Ob Validierung und Fehlerbehandlung für kurze oder unvollständige Pipe-Nachrichten erweitert werden sollte
