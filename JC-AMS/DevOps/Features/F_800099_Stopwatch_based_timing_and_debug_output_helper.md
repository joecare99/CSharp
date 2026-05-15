# Feature: Bietet eine Stoppuhr für Timing- und Debug-Ausgaben

## Beschreibung

Dieses Feature beschreibt `CStopuhr` als kleine Timing-Hilfsklasse auf Basis von `Stopwatch` mit Debug-Ausgabe.

## Sichtbare technische Bausteine

- `Core\Core\Components\CStopuhr.cs`
- `Core\Core\Logging\SLogging.cs`

## Fachlicher Nutzen

- Laufzeiten können schnell gemessen werden
- Debug-Ausgaben erhalten einen einheitlichen Zeitkontext
- Prozentuale Auswertung gegenüber einem Zielintervall ist möglich

## Beobachtete Abläufe

- Der Konstruktor startet direkt die interne Stoppuhr.
- `Info(...)` liefert formatierte Millisekunden- oder Prozentwerte.
- `DebugPrintInfo(...)` stoppt, loggt und startet die Messung wieder.

## Offene Fragen

- Ob die Prozentformel noch den gewünschten Einheitenbezug hat
- Ob die Klasse für längere Messreihen erweitert werden sollte
