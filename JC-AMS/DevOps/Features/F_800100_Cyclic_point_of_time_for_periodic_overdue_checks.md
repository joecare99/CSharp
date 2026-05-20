# Feature: Bietet zyklische Zeitpunktprüfung für periodische Fälligkeit

## Beschreibung

Dieses Feature beschreibt `CCyclicPointOfTime` als Hilfsklasse zur Prüfung, ob ein zyklischer Zeitpunkt überfällig ist.

## Sichtbare technische Bausteine

- `Core\Core\Components\CCyclicPointOfTime.cs`
- Zeitlogik auf Basis von `DateTime` und `TimeSpan`

## Fachlicher Nutzen

- Periodische Abläufe können einfach auf Fälligkeit geprüft werden
- Startzeit und Zyklus werden kompakt modelliert
- Überfällige Zyklen können nachgeholt bzw. auf den letzten Zykluspunkt zurückgeführt werden

## Beobachtete Abläufe

- Die Konstruktoren setzen den letzten Lauf auf den heutigen Tagesanfang plus Startzeit.
- `CycleOverdue(...)` prüft die Zyklusdauer gegen die aktuelle Zeit.
- Bei Überfälligkeit wird der letzte Lauf schrittweise nachgezogen.

## Offene Fragen

- Ob die Zeitlogik bei Tageswechseln oder sehr langen Unterbrechungen weiter abgesichert werden sollte
- Ob die Klasse in einem Scheduler- oder Timer-Kontext wiederverwendet wird
