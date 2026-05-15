# Feature: Sendet IPC-Kommandos, beantwortet Pings und verfolgt Empfangszeiten

## Beschreibung

Dieses Feature beschreibt die Funktionsseite von `CIPC` für Named-Pipe-Kommunikation. Die Klasse sendet strukturierte Kommandos, behandelt Ping-Antworten und kann die letzten Empfangszeitpunkte je Zielinstanz auswerten.

## Sichtbare technische Bausteine

- `Core\Core\Components\CIPC.cs`
- `Core\Core\Communication\NamedPipes\CNamedPipe.cs`
- `Core\Core\Communication\NamedPipes\CNamedPipeReadEventArgs.cs`
- `Core\Core\CommandSystem\CommandStatement.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- IPC-Kommandos werden in einem festen Telegrammformat verschickt
- Ping-Zyklen können automatisch fortgesetzt werden
- Die letzte Kommunikationsaktivität pro Zielinstanz ist auswertbar

## Beobachtete Abläufe

- `Send(...)` baut ein Texttelegramm mit Zeit, Instanz-IDs, Command und Parametern.
- Parameter werden mit `^` verknüpft und Zeilenumbrüche für das Telegramm neutralisiert.
- `AnswerPing(...)` reduziert einen Zähler und sendet bei Bedarf einen Folge-Ping.
- `GetLastReceiveIO(...)` ermittelt den jüngsten Empfangszeitpunkt für eine Zielinstanz.
- `Delete()` räumt alle angelegten Named Pipes auf.

## Offene Fragen

- Ob die Telegrammformatierung künftig stärker typisiert oder versioniert werden sollte
- Ob die Instanz- und Mode-Entscheidungen in eigene Strategieobjekte ausgelagert werden sollten
