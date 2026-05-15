# Feature: Modelliert Wartungsaufgaben mit Terminfortschreibung

## Beschreibung

Dieses Feature beschreibt `CMaintenanceTask` als Task-Modell mit Fälligkeit, Substation-Zuordnung und Fortschreibungslogik.

## Sichtbare technische Bausteine

- `Core\Core\Components\Maintenance\CMaintenanceTask.cs`
- `Core\Core\Components\Maintenance\CMaintenanceJob.cs`
- `Core\Core\SQL\CSQLQuery.cs`
- `Core\Core\SQL\TSQLHelpers.cs`

## Fachlicher Nutzen

- Wartungsaufgaben können einzeln persistiert und aktualisiert werden
- Terminfortschreibungen orientieren sich am zugehörigen Job
- Task und Job bleiben getrennte fachliche Objekte

## Beobachtete Abläufe

- `Insert()`, `Update()` und `Delete()` arbeiten direkt gegen die Datenbanktabelle.
- `Increment()` verschiebt die Fälligkeit abhängig vom Job-Zyklus.
- Bei langen Zyklen wird der nächste Termin auf den Tageszeitpunkt des Erstservices gesetzt.

## Offene Fragen

- Ob die Fortschreibungslogik für Tages- und Untertageszyklen noch vollständig robust ist
- Ob die Feldbasiertheit in modernere Properties überführt werden sollte
