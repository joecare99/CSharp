# Feature: Plant Wartungsjobs, -tasks und Protokolle zentral

## Beschreibung

Dieses Feature beschreibt `CMaintenance` als Wartungskoordinator für Jobs, Tasks und Protokolle. Die Klasse wertet Datenbankzustände aus und stößt bei Bedarf automatische Wartungsaktionen an.

## Sichtbare technische Bausteine

- `Core\Core\Components\Maintenance\CMaintenance.cs`
- `Core\Core\Components\Maintenance\CMaintenanceJob.cs`
- `Core\Core\Components\Maintenance\CMaintenanceTask.cs`
- `Core\Core\Components\Maintenance\CMaintenanceProtocol.cs`
- `Core\Core\Components\CTimerState.cs`
- `Core\Core\Messaging\ErrorHandling\CErrorManager.cs`

## Fachlicher Nutzen

- Wartungsarbeiten können als Jobs und Tasks geplant werden
- Automatische Ausführung und sichtbare Aufgabenlisten bleiben getrennt
- Fällige Wartung kann Fehlerzustände und Commands auslösen
- Protokolle dokumentieren durchgeführte Wartungsaktionen

## Beobachtete Abläufe

- `Refresh()` schützt sich über `CTimerState` gegen parallele Ausführung.
- `GetActualJobsToExecute()` sucht nach sofort fälligen Jobs und Tasks.
- `GetActualJobsToDisplay()` lädt Aufgaben zur Anzeige und setzt Fehlerzustände für zyklische Wartung.
- Die Klasse verwendet Datenbank-Views und Instanzbezug, um gültige Einträge zu bestimmen.

## Offene Fragen

- Ob die Wartungslogik weiter in Anzeige, Ausführung und Persistenz getrennt werden sollte
- Ob die Datenbank-Views noch die vollständige fachliche Grundlage bilden
