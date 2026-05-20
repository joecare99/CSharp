# Feature: Verbindet sich mit einer SPS

## Beschreibung

Die Solution enthält mehrere Bausteine für die Anbindung an eine SPS/PLC. Dazu gehören Kommunikationsobjekte, AGV-bezogene Steuerungslogik, Bedienoberflächen und zentrale Systemzustände.

## Sichtbare technische Bausteine

- `Core\Core\AGV`: Handler, Sicherheitsobjekte, Fahrzeug- und Transporttypen
- `Core\Core\Communication` und `Core\Core\CommSystem`: Telegramme, Kommunikationspartner, Kommunikationsstationen
- `PLC\JCAMS.PLC.csproj`: separates PLC-nahes Projekt
- `WinGUI\WinGUI`: Steuerungs- und Diagnose-Controls
- `MPS\MPS\CMPS.cs`: Startet Domänenobjekte und bindet Datenquellen ein

## Fachlicher Nutzen

- Zustände der Anlage werden strukturiert erfasst
- Bedienaktionen können in Steuerbefehle umgesetzt werden
- Sicherheits- und Handlerinformationen bleiben fachlich getrennt
- Die Anbindung kann projektübergreifend wiederverwendet werden

## Offene Fragen

- Welche PLC-Protokolle produktiv eingesetzt werden
- Welche Signale zyklisch gelesen und welche aktiv geschrieben werden
- Wie Fehler und Kommunikationsabbrüche zwischen UI, Service und PLC gehandhabt werden
