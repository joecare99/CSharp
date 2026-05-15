# Feature: Bietet Datei- und Verzeichnis-Hilfen

## Beschreibung

Dieses Feature beschreibt `SFileHelpers` als zentrale Hilfsklasse für Datei-, Verzeichnis- und Pfadprüfungen im Core-Bereich.

## Sichtbare technische Bausteine

- `Core\Core\SFileHelpers.cs`
- `Core\Core\System\SConfiguration.cs`
- `Core.Tests`-nahe Datei- und Pfadkontexte

## Fachlicher Nutzen

- Verzeichnisse können bei Bedarf automatisch angelegt werden
- Datei- und Pfadprüfungen bleiben an einer Stelle gebündelt
- Verzeichnisinhalte können rekursiv bereinigt werden
- Sicherheits- und Zugriffsanpassungen sind vorbereitet

## Beobachtete Abläufe

- `CheckAndCreateDirectory(...)` erkennt Sondermarker und legt Verzeichnisse im Startup-Kontext an.
- `CheckFileExists(...)` prüft per `FileInfo`, ob eine Datei existiert.
- `CheckPathAccessibility(...)` validiert lokale und Netzwerkpfade.
- `DeleteAllSubfolders(...)` und `DeleteDirectory(...)` entfernen Dateien und Unterverzeichnisse.

## Offene Fragen

- Ob die leeren Security-Hilfsmethoden künftig noch implementiert werden sollen
- Ob die Pfadvalidierung robuster auf kurze oder ungültige Eingaben reagieren sollte
