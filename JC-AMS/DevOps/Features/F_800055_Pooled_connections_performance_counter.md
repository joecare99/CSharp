# Feature: Zählt gepoolte Verbindungen als Performance-Counter

## Beschreibung

Dieses Feature beschreibt `CPooledConnections` als kleine Performance-Counter-Klasse für die Anzahl gepoolter Verbindungen.

## Sichtbare technische Bausteine

- `Core\Core\Communication\CPooledConnections.cs`
- `Core\Core\PerformanceMeasure\CPerformanceCounter.cs`
- `Core\Core\System\SJCAMS.cs`

## Fachlicher Nutzen

- Pooled-Connection-Zahlen können über einen standardisierten Performance-Counter beobachtet werden
- Kommunikations- und Datenanbieter-Kontexte lassen sich im Monitoring unterscheiden
- Die Klasse unterstützt Betriebsdiagnose ohne zusätzliche Logik

## Beobachtete Abläufe

- Der Konstruktor initialisiert den Basis-Counter mit Name, Kategorie und Instanzpfad.
- Die Klasse erweitert nur den PerformanceCounter-Basistyp und fügt Naming-Konventionen hinzu.

## Offene Fragen

- Ob weitere Kommunikationsmetriken als eigene Counter-Klassen dokumentiert werden sollen
- Ob die Counter-Namenskonventionen konsolidiert werden sollten
