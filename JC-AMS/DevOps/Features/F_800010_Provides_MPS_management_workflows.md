# Feature: Bietet MPS-Verwaltungs- und Bearbeitungsworkflows

## Beschreibung

Dieses Feature beschreibt die Verwaltungslogik für MPS-Objekte wie Paletten, Werkstücke, Werkzeuge, Plätze, NC-Programme und Produktionszellen. Der MPS.Manager-Bereich enthält dafür umfangreiche Bearbeitungs- und Visualisierungsmasken.

## Sichtbare technische Bausteine

- `MPS.Manager\MPS\Manager\CMPSManagerSystem.cs`: Start, PLC-Anbindung, zyklische Verarbeitung
- `MPS.Manager\MPS\Manager\Form*`, `Ctrl*`, `Panel*`: Bearbeitung und Darstellung der MPS-Daten
- `MPS\MPS\CMPS.cs`: Initialisierung und Aktualisierung der MPS-Bausteine

## Fachlicher Nutzen

- Produktionsdaten können verwaltet und visualisiert werden
- Paletten, Werkzeuge und Werkstücke lassen sich strukturiert bearbeiten
- Workflows für NC-Programme und Produktionszellen werden bedienbar
- Prüfung von Konfiguration und Plausibilität wird unterstützt

## Offene Fragen

- Welche Bearbeitungsabläufe die Anwender regelmäßig benötigen
- Welche Daten direkt editierbar und welche nur lesbar sind
- Wie MPS, PLC und DNC im UI konsistent zusammenarbeiten
