# Feature: Steuert Busy-/Idle-Zustände für zeitkritische Abläufe

## Beschreibung

Dieses Feature beschreibt `CTimerState` als kleine Zustandsklasse zur Synchronisation von Busy- und Idle-Phasen.

## Sichtbare technische Bausteine

- `Core\Core\Components\CTimerState.cs`
- `System.Threading.Interlocked`

## Fachlicher Nutzen

- Zeitkritische Timer- oder Hintergrundabläufe können gegen Mehrfachzugriffe geschützt werden
- Busy-/Idle-Wechsel bleiben atomar und leicht verständlich
- Die Klasse eignet sich für einfache nebenläufige Zustandskontrolle

## Beobachtete Abläufe

- Der Konstruktor startet im Idle-Zustand.
- `IsBusy` prüft und wechselt per Interlocked-Operationen auf Busy.
- `SetIdle()` und `SetBusy()` setzen den Zustand atomar zurück oder um.

## Offene Fragen

- Ob die Busy-/Idle-Semantik in Zukunft durch ein robusteres State-Enum ersetzt werden sollte
- Ob zusätzliche Testabdeckung für Race-Conditions vorgesehen werden soll
