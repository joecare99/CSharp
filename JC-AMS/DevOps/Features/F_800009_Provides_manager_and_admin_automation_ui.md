# Feature: Stellt Manager- und Administrationsoberflächen bereit

## Beschreibung

Dieses Feature umfasst die zentrale Verwaltungsoberfläche für Benutzer, Systeme, Kommunikation, Diagnose und Wartung. Die Manager-Projekte enthalten dafür zahlreiche Formulare, Panels und Steuerungsfunktionen.

## Sichtbare technische Bausteine

- `Manager\Manager\CManager.cs`: zentrale Manager-Klammer für Form und Autorisierung
- `Manager\Manager\Form*`: Benutzerverwaltung, Systemansichten, Logs, Kommunikation und Wartung
- `MPS.Manager\MPS\Manager\*`: MPS-bezogene Verwaltungs- und Bearbeitungsoberflächen

## Fachlicher Nutzen

- Bediener und Administratoren erhalten zentrale Werkzeuge
- Systemzustände, Logs und Konfigurationen werden sichtbar
- MPS-, PLC- und DNC-Daten können bearbeitet und geprüft werden
- Verwaltungsschritte bleiben getrennt von der Fachlogik

## Offene Fragen

- Welche Masken produktiv tatsächlich verwendet werden
- Wie Rechte und Autorisierungen im Detail greifen
- Welche Formulare nur Diagnose und welche echte Pflegefunktionen übernehmen
