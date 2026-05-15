# Feature: Bietet Debug-Hilfen für Method-Stack-Ermittlung

## Beschreibung

Dieses Feature beschreibt `SDebugHelpers` als kleine Debug-Hilfsklasse zur Ermittlung des aktuellen Methodenpfads über den Stack.

## Sichtbare technische Bausteine

- `Core\Core\SDebugHelpers.cs`
- `System.Diagnostics.StackTrace`
- `System.Reflection.MethodBase`

## Fachlicher Nutzen

- Methodenketten können für Logging oder Diagnose kompakt erfasst werden
- Fehlerdiagnosen erhalten eine lesbare Kontextinformation
- Der Stack wird auf eine begrenzte Tiefe reduziert

## Beobachtete Abläufe

- `GetMethodStack()` liest bis zu zehn Stackframes aus.
- Die Methodennamen werden in umgekehrter Reihenfolge mit Punkten verknüpft.
- Fehler beim Auslesen werden geloggt und unterbrechen den Ablauf nicht.
- Die zugehörigen Tests prüfen eine konkrete erwartete Stackdarstellung.

## Offene Fragen

- Ob die Ausgabe künftig flexibler formatiert oder aufgelockert werden sollte
- Ob weitere Debug-Hilfen in dieselbe Dokumentationsgruppe gehören
