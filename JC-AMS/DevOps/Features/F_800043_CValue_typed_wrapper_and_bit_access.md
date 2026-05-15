# Feature: Bietet typisierte Wrapper- und Bitzugriffe auf Systemwerte

## Beschreibung

Dieses Feature beschreibt `CValue` als Wrapper um `CSystemValue`. Die Klasse stellt typisierte Getter und Setter, Bitzugriffe und Property-Change-Weiterleitung für UI-nahe Verwendungen bereit.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CValue.cs`
- `Core\Core\System\Values\CSystemValue.cs`
- `Core\Core\System\Values\CValuePropertyDescriptor.cs`

## Fachlicher Nutzen

- Systemwerte werden als bool-, numerische, Datums- und String-Eigenschaften nutzbar
- Bitzugriffe können direkt über einen indexierten Zugriff erfolgen
- Änderungen am inneren Systemwert werden nach außen weitergereicht
- Der Wrapper eignet sich für dynamische Anzeige- und Bindungsszenarien

## Beobachtete Abläufe

- `GetValueAsType<T>()` und `SetValueAsType<T>()` kapseln die Typumwandlung.
- `CheckValidity()` lädt den Systemwert bei Bedarf nach.
- `Bit` bietet einen schreibbaren Bit-Indexer.
- `ToString()` erzeugt eine kompakte Statusdarstellung mit Wert und Zuordnung.

## Offene Fragen

- Ob die Wrapper-API stärker vereinheitlicht werden soll
- Ob fehlende Lookup-Implementierungen in den Basisklassen die Nutzung weiter einschränken
