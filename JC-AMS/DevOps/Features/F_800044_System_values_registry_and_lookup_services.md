# Feature: Verwaltet Systemwerte zentral in einer Registry

## Beschreibung

Dieses Feature beschreibt `CSystemValues` als zentrale Registry für erweiterte Systemwerte. Die Klasse bündelt Lookup-, Konvertierungs- und Refresh-Logik für stationäre Werte.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CSystemValues.cs`
- `Core\Core\System\Values\CSystemValueExt.cs`
- `Core\Core\CommSystem\SSystemValuesHelpers.cs`
- `Core\Core\Extensions\SAsBoolXtntn.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`

## Fachlicher Nutzen

- Systemwerte sind zentral auffindbar und typisiert abrufbar
- Bool-, Integer-, Double- und String-Lookups werden konsistent angeboten
- Ein periodischer Refresh hält die Registry aktuell
- Die Registry ist Grundlage für Synchronisations- und Kommunikationslogik

## Beobachtete Abläufe

- Eine statische Dictionary-/Index-Struktur hält die registrierten Werte.
- Ein Timer triggert periodisch `SSystemValuesHelpers.Read()`.
- `GetSystemValue(...)` sucht per UID und liefert registrierte Werte zurück.
- Mehrere `As...`-Helfer konvertieren Werte direkt aus dem Registry-Kontext.

## Offene Fragen

- Ob die Registry-Suche robustere Fehlerbehandlung für inkonsistente Indizes benötigt
- Ob der Refresh-Timer künftig konfigurierbar sein sollte
