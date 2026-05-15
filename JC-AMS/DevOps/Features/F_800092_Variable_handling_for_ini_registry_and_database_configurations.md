# Feature: Verwaltet Variablen aus Ini-, Registry- und Datenbankquellen

## Beschreibung

Dieses Feature beschreibt `SVariableHandling` als Konfigurationsschicht zum Laden und Speichern von Werten aus unterschiedlichen Quellen.

## Sichtbare technische Bausteine

- `Core\Core\SVariableHandling.cs`
- `Core\Core\System\SConfiguration.cs`
- `Core\Core\System\CConfiguration_Mode.cs`
- `Core\Core\SQL\CSQLQuery.cs`
- `Microsoft.Win32.Registry`

## Fachlicher Nutzen

- Konfigurationswerte können quellenübergreifend geladen werden
- Instanzbezogene Werte werden unterstützt
- Bool-, Int-, Double-, Decimal- und DateTime-Werte werden typisiert behandelt
- Startup- und Datenbanklogik bleiben an einer Stelle gebündelt

## Beobachtete Abläufe

- `LoadInstancedVar(...)` unterscheidet zwischen Ini- und Datenbankquelle.
- `LoadVar(...)` bietet mehrere Überladungen für unterschiedliche Zieltypen.
- Fehlende Konfigurationen können im Startup-Modus automatisch angelegt werden.
- Die Klasse ist eng mit `SJCAMS`, `SConfiguration` und SQL-Kontexten verknüpft.

## Offene Fragen

- Ob die Quellenauswahl stärker abstrahiert werden sollte
- Ob das Verhalten beim automatischen Anlegen fehlender Werte weiterhin beabsichtigt ist
