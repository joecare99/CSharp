# Feature: Integriert PLC-Zustände und Betriebsarten

## Beschreibung

Dieses Feature beschreibt die PLC-nahe Logik für Zustände, Betriebsarten und zyklische Verarbeitung. Die Solution enthält dafür eigene PLC-Klassen, Steuerungs-Controls und Datenobjekte.

## Sichtbare technische Bausteine

- `PLC\PLC\CBasePLC.cs`: zentrale PLC-Basis mit Kommunikations-, Timer- und Zustandslogik
- `PLC\PLC\CSiemensPLC.cs`, `CMoviPLC.cs`: konkrete PLC-Implementierungen
- `PLC\PLC\Ctrl*` und `Form*`: Bedien- und Diagnoseoberflächen
- `Core\Core\CommSystem` und `Core\Core\Communication`: Kommunikationsbasis für Telegramme und Verbindungen

## Fachlicher Nutzen

- Betriebsarten und Maschinenzustände können zentral geführt werden
- PLC-Kommunikation ist für verschiedene Hersteller abstrahiert
- Bedienoberflächen können Status und Eingaben konsistent darstellen
- Zyklen, Timer und Requests sind klar strukturiert

## Offene Fragen

- Welche PLC-Hersteller dauerhaft unterstützt werden
- Wie Sicherheits- und Referenzierungsprozesse genau ablaufen
- Welche Zustände direkt aus der PLC kommen und welche lokal berechnet werden
