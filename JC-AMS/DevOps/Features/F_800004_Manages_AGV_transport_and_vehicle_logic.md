# Feature: Verarbeitet AGV-Transport- und Fahrzeuglogik

## Beschreibung

Dieses Feature beschreibt die fahrzeug- und transportbezogene Logik für AGV-Szenarien. Die Solution enthält dafür eigene AGV-Klassen, Produktionslogiken, Sicherheitsreaktionen und Servicekomponenten.

## Sichtbare technische Bausteine

- `AGV\AGV\CAGV.cs`: Initialisierung von Fahrzeug-, Signal- und Sicherheitspools
- `AGV\AGV\CVehicle.cs`, `CVehicleLogic.cs`, `CInstruction*`: Fahrzeug- und Befehlsverarbeitung
- `AGV\AGV\Service`: AGV-nahe Serviceobjekte und HTTP-Service
- `Core\Core\AGV`: gemeinsame AGV-Domänenobjekte und Schnittstellen

## Fachlicher Nutzen

- Transportaufträge können fahrzeuggerecht verarbeitet werden
- Sicherheitszustände werden zentral berücksichtigt
- Verschiedene Lager- und Produktionsstrategien können über Logikklassen abgebildet werden
- Fahrzeugtypen und Umgebungslogik bleiben voneinander trennbar

## Offene Fragen

- Welche AGV-Typen produktiv eingesetzt werden
- Wie Instruktionen in konkrete Fahrbefehle übersetzt werden
- Wie Sicherheitsverletzungen in die Produktionslogik zurückwirken
