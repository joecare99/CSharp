# Feature: Stellt Prozess-Hilfen für Start und Capture bereit

## Beschreibung

Dieses Feature beschreibt die Prozess-Hilfen im Core, mit denen externe Programme gestartet, vorhandene Fenster geprüft und Ausgaben eingefangen werden können.

## Sichtbare technische Bausteine

- `Core\Core\SProcessHelpers.cs`

## Fachlicher Nutzen

- Externe Tools und Systembefehle können zentral gestartet werden
- Laufende Prozesse lassen sich anhand von Namen und Fenstertiteln erkennen
- Standardausgaben können für Diagnose oder weitere Verarbeitung eingefangen werden
- Die Prozesslogik bleibt von den aufrufenden Fachbereichen getrennt

## Beobachtete Abläufe

- `GetProcessesByNameAndTitle()` sucht passende Prozesse nach Titeltexten.
- `Start()` wird in mehreren Überladungen für einfache Starts, Starts mit Rückgabe und Start mit Zeitlimit genutzt.
- `Capture()` startet ein Programm mit umgeleiteter Ausgabe und sammelt die Standardausgabe.
- Fehler werden zentral über `SLogging` protokolliert.

## Offene Fragen

- Ob die Prozess-Utilities in eine modernere Abstraktionsschicht überführt werden sollen
- Welche Startfälle produktiv wirklich benötigt werden
- Ob Capture-Ausgaben noch zusätzlich testbar gemacht werden müssen
