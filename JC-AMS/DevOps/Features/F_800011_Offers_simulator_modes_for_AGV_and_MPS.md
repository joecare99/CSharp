# Feature: Bietet Simulator-Modi für AGV und MPS

## Beschreibung

Dieses Feature beschreibt die Simulationsanwendungen für AGV- und MPS-Szenarien. Die Lösung enthält dafür eine eigene Simulator-App mit unterschiedlichen Startmodi.

## Sichtbare technische Bausteine

- `Simulator\Simulator\Program.cs`: Startet je nach Basismodus AGV- oder MPS-Simulation
- `Simulator\Simulator\frmMain*`, `FormSPCSimulator`: Simulationsoberflächen
- `Simulator\Simulator\AGV\*`: AGV-spezifische Simulationsobjekte

## Fachlicher Nutzen

- Betriebsszenarien können ohne reale Hardware geprüft werden
- AGV- und MPS-Fälle lassen sich getrennt simulieren
- Bedien- und Diagnosewege können vorab getestet werden
- Störungen und Zustandsübergänge werden reproduzierbar

## Offene Fragen

- Wie weit die Simulation die reale Anlage abbildet
- Welche Datenquellen für Simulationszustände verwendet werden
- Welche Szenarien automatisiert getestet werden sollen
