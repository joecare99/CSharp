# Feature: Modelliert Wartungsprotokolle und Pending-Lookups

## Beschreibung

Dieses Feature beschreibt `CMaintenanceProtocol` als Protokollmodell für geplante und ausgeführte Wartungen mit Pending-Abfrage und Insert-Logik.

## Sichtbare technische Bausteine

- `Core\Core\Components\Maintenance\CMaintenanceProtocol.cs`
- `Core\Core\Components\Maintenance\CMaintenanceTask.cs`
- `Core\Core\Components\Maintenance\CMaintenanceJob.cs`
- `Core\Core\Globalisation\TTranslations.cs`
- `Core\Core\SQL\CSQLQuery.cs`

## Fachlicher Nutzen

- Geplante Wartungen können protokolliert und später ausgewertet werden
- Pending-Protokolle können anhand eines Jobs gefunden werden
- Signaturen und Annotationen bleiben Teil des Protokolls

## Beobachtete Abläufe

- Konstruktoren laden Protokolle aus Datenbank oder Task-Kontext.
- `Q2C(...)` füllt die Protokolldaten und lädt den zugehörigen Job.
- `Save()` aktualisiert vorhandene Einträge oder erzeugt neue.
- `Insert()` schreibt einen ausgeführten Wartungseintrag mit Signatur und Übersetzungstext.
- `GetPendingProtocol(...)` liefert noch ausstehende Protokolle für eine Wartung.

## Offene Fragen

- Ob die Übersetzungsermittlung für den Standardkommentar noch aktuell ist
- Ob die Klasse stärker gegen leere Signaturen oder Annotationen abgesichert werden sollte
