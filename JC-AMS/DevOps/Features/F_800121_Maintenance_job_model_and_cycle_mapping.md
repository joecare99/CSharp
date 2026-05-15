# Feature: Modelliert Wartungsjobs mit Zyklus- und Command-Mapping

## Beschreibung

Dieses Feature beschreibt `CMaintenanceJob` als Modell für Wartungsaufgaben mit Zyklus, Beschreibung, Kommando und Instanzbezug.

## Sichtbare technische Bausteine

- `Core\Core\Components\Maintenance\CMaintenanceJob.cs`
- `Core\Core\SQL\CSQLQuery.cs`
- `Core\Core\SQL\TSQLHelpers.cs`

## Fachlicher Nutzen

- Wartungsjobs können aus der Datenbank geladen und gespeichert werden
- Zyklusinformationen lassen sich aus unterschiedlichen Zyklusdefinitionen ableiten
- Commands und Sichtbarkeit bleiben Teil des Modells

## Beobachtete Abläufe

- Der Konstruktor lädt einen Job über `IdMaintenance` oder aus einer Query.
- `Q2C(...)` liest Felder wie Beschreibung, LongDescription, FirstService und Command.
- `Cycle_sec` mappt definierte Zykluskennzahlen auf konkrete Sekundenwerte.
- `Save()` aktualisiert vorhandene Einträge oder legt neue an.

## Offene Fragen

- Ob die zyklischen Mappings von ID auf Sekunden weiterhin hart kodiert bleiben sollen
- Ob die String-basierte SQL-Erzeugung langfristig ersetzt werden sollte
