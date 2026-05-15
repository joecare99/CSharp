# Feature: Startet den JCAMS-Manager und steuert den Startup-Flow

## Beschreibung

Dieses Feature beschreibt den Applikationsstart des zentralen Managers. Der Startup-Flow initialisiert Systemzustände, prüft Mehrfachstarts, wartet auf Verzögerungen, startet die Oberfläche und behandelt Fehler sowie Lizenzprüfungen.

## Sichtbare technische Bausteine

- `JCAMSManager\Manager\Program.cs`: Einstiegspunkt der Anwendung
- Splash-Screen- und Retry-Logik
- Initialisierung von `SJCAMS`
- Auswahl des passenden Manager-Systems je nach Basismodus

## Fachlicher Nutzen

- Der Start wird kontrolliert und nachvollziehbar ausgeführt
- Fehler werden früh erkannt und dem Benutzer angezeigt
- Mehrfachstarts und Lizenzprobleme werden verhindert
- Verschiedene Betriebsarten können über denselben Einstiegspunkt gestartet werden

## Offene Fragen

- Welche Startparameter im Alltag tatsächlich verwendet werden
- Wie oft Startup-Verzögerungen oder Retry-Schleifen gebraucht werden
- Welche Teile des Startprozesses künftig modernisiert werden sollen
