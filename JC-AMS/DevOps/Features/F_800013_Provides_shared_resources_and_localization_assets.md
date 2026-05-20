# Feature: Stellt gemeinsame Ressourcen und Lokalisierungsdaten bereit

## Beschreibung

Dieses Feature beschreibt die zentralen Ressourcen der Solution. Dazu gehören Bilder, Icons und weitere UI-nahe Assets, die von mehreren Projekten verwendet werden können.

## Sichtbare technische Bausteine

- `Resources\Properties\Resources.designer.cs`: stark typisierte Ressourcenklasse
- Bild- und Symbolressourcen für AGV-, Kommunikations- und UI-Ansichten
- Nutzung über `JCAMS.Resources` hinweg

## Fachlicher Nutzen

- UI-Elemente können einheitlich gestaltet werden
- Wiederverwendbare Grafiken und Icons werden zentral gepflegt
- Mehrere Projekte können dieselben visuellen Assets nutzen
- Ressourcenverwaltung bleibt von Fachlogik getrennt

## Offene Fragen

- Welche Ressourcen noch nicht dokumentiert sind
- Welche Bilder nur historisch und welche produktiv genutzt werden
- Ob weitere Lokalisierungsdaten neben Bildern vorhanden sind
