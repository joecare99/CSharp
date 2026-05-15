# Feature: Kapselt Systemwerte als property-fähige Werte

## Beschreibung

Dieses Feature beschreibt die Wrapper- und Collection-Klassen für Systemwerte. Sie machen einzelne Systemwerte als typisierte Eigenschaften verfügbar und erlauben die dynamische Darstellung in Property-Umgebungen.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CValue.cs`
- `Core\Core\System\Values\CValueCollection.cs`
- `Core\Core\System\Values\CSystemValue.cs`
- `Core\Core\System\Values\CSystemValueDef.cs`

## Fachlicher Nutzen

- Systemwerte können über bool-, numerische, Datums- und String-Zugriffe angesprochen werden
- Bitzugriffe werden als eigene Hilfsstruktur bereitgestellt
- Sammlungen lassen sich über `ICustomTypeDescriptor` in UI-/Designer-Kontexten verwenden
- Property-Change-Weiterleitung bleibt für gebundene Oberflächen sichtbar

## Beobachtete Abläufe

- `CValue` delegiert an `CSystemValue` und bietet typisierte Getter/Setter.
- `CValue.Bit` erlaubt Bitzugriffe über einen indexierten Zugriff.
- `CValueCollection` verwaltet `CValue`-Instanzen in einer `CollectionBase`-Sammlung.
- Die Collection erstellt `CValuePropertyDescriptor`-Objekte für dynamische Eigenschaften.

## Offene Fragen

- Ob die Wrapper-Klasse noch auf modernere Binding-Mechanismen umgestellt werden soll
- Ob die derzeit teils obsolete Konstruktorlogik vereinfacht werden kann
