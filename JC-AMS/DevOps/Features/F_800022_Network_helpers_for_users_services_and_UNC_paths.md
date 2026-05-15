# Feature: Stellt Netzwerk-Hilfen für Benutzer, Services und UNC-Pfade bereit

## Beschreibung

Dieses Feature beschreibt die Netzwerk- und Umgebungshilfen für Benutzerprüfung, UNC-/Netzlaufwerke, Remote-Services und Computernetz-Erkennung.

## Sichtbare technische Bausteine

- `Core\Core\SNetHelpers.cs`
- mögliche zugehörige Testabdeckung im Core-Testbereich

## Fachlicher Nutzen

- Benutzerexistenz kann gegen lokale oder Domain-Listen geprüft werden
- UNC-Pfade und Netzlaufwerke lassen sich verbinden, trennen und auf Verfügbarkeit prüfen
- Remote-Dienste können auf ihren Status abgefragt werden
- Netzwerk-Umgebungen lassen sich für Start- und Betriebslogik prüfen

## Beobachtete Abläufe

- Lokale und Domänen-User werden über `NET USER` ermittelt.
- UNC-Verbindungen werden über `NET USE` hergestellt und geprüft.
- Netzwerkfreigaben und Remote-Service-Status sind stark prozess- und Betriebssystem-nah.
- Fehler werden zentral über `SLogging` protokolliert.

## Offene Fragen

- Welche dieser Helfer im produktiven Betrieb tatsächlich verwendet werden
- Ob die Prozessaufrufe modernisiert oder abstrahiert werden sollen
- Welche Testabdeckung für Netzwerk- und Umgebungshilfen fehlt
