# Feature: Bietet FTP-Hilfen für Dateiübertragung und veraltete Dateien

## Beschreibung

Dieses Feature beschreibt `SFTPHelper` als Hilfsklasse für FTP-Downloads, Abschlussprüfungen und das Verschieben bzw. Löschen veralteter Dateien.

## Sichtbare technische Bausteine

- `Core\Core\SFTPHelper.cs`
- `Core\Core\Services\FTP\CFtpClient.cs`
- `Core\Core\Services\FTP\CFtpClient2.cs`

## Fachlicher Nutzen

- Dateien können per FTP geladen werden
- Upload-Abschluss kann über Dateisperren geprüft werden
- Veraltete Dateien können nach Alter gefiltert und verschoben werden

## Beobachtete Abläufe

- `FileUploadCompleted(...)` prüft, ob eine Datei exklusiv geöffnet werden kann.
- `FTPGet(...)` lädt eine Datei per `FtpWebRequest` herunter und speichert sie lokal.
- `CopyFilesOlderThanSeconds(...)` und `MoveFilesOlderThanSeconds(...)` bilden ein FTP-Workflow-Gerüst für Dateiaufräumaufgaben.

## Offene Fragen

- Ob die fest verdrahtete FTP-Serveradresse in `CopyFilesOlderThanSeconds(...)` noch aktuell ist
- Ob die Dateioperationen für Fehlerfälle robuster abgesichert werden sollten
