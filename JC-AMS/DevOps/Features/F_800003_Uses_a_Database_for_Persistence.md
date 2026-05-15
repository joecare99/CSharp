# Feature: Nutzt eine Datenbank für Persistenz

## Beschreibung

Die Solution nutzt eine Datenbank zur dauerhaften Speicherung von Konfiguration, Betriebsdaten und historischen Informationen. Dafür existieren Hilfsklassen im Core, Initialisierungslogik im MPS-Bereich und Dienststartlogik im Service.

## Sichtbare technische Bausteine

- `Core\Core\System\CDatabaseIO.cs`
- `Core\Core\SQL`: SQL-Hilfen und Abfragen
- `Service\Service\CService.cs`: Dienststart, Retry-Logik, Hostdaten
- `MPS\MPS\CMPS.cs`: Initialisierung von Produktions- und Stammdatenpools
- `FASys\KG.MPS.Interface`: Schnittstelle für Ressourcen- und Dateioperationen

## Fachlicher Nutzen

- Konfiguration bleibt nach Neustart erhalten
- Produktionsobjekte können wiederhergestellt werden
- Stammdaten und Laufzeitdaten werden getrennt behandelbar
- Historische Auswertungen und Nachverfolgung werden ermöglicht

## Offene Fragen

- Welche Tabellen und Entitäten tatsächlich produktiv genutzt werden
- Welche Daten synchron und welche asynchron gespeichert werden
- Wie Archivierung und Löschung historischer Daten geregelt sind
