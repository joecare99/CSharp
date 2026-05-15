# Feature: Normalisiert Control-Namen und erkennt Steuerelementtypen

## Beschreibung

Dieses Feature beschreibt `SControlNameHelper` als Hilfsklasse zur Trennung von Kontrollnamen, Typpräfixen und Benennungstermini.

## Sichtbare technische Bausteine

- `Core\Core\SControlNameHelper.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`
- `Core\Core\Logging\SLogging.cs`

## Fachlicher Nutzen

- UI-Namen können konsistent normalisiert werden
- Präfixe liefern Hinweise auf den tatsächlichen Steuerelementtyp
- Der fachliche Kontrollname bleibt ohne technische Präfixe verfügbar

## Beobachtete Abläufe

- `ControlTermAndType(...)` entfernt numerische Suffixe und analysiert dann bekannte Präfixe.
- Eine Prefix-Tabelle ordnet Kürzel wie `btn`, `lbl` oder `pnl` einem Typ zu.
- `TrimControlName(...)` liefert den bereinigten Namen zurück.

## Offene Fragen

- Ob die Prefix-Liste weiter standardisiert oder projektübergreifend angepasst werden sollte
- Ob die Typnamen in der Zuordnung teilweise noch vereinheitlicht werden müssen
